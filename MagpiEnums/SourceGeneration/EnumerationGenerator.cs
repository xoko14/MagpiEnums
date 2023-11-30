using System.Text;
using MagpiEnums.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagpiEnums.SourceGeneration;

[Generator]
public class EnumerationGenerator: ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new EnumerationSyntaxReciever());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var syntaxReceiver = context.SyntaxReceiver as EnumerationSyntaxReciever;

        var targetRecord = syntaxReceiver?.TargetRecord;
        if(targetRecord is null)
            return;

        var isAbstract = targetRecord.Modifiers.Any(m => m.ToString() == "abstract");
        var isPartial = targetRecord.Modifiers.Any(m => m.ToString() == "partial");

        if (!(isAbstract && isPartial))
        {
            return;
        }
        
        var enumerationAttribute = targetRecord.FindAttribute("Enumeration");
        var discriminatorPropertyName = enumerationAttribute?.ArgumentList?
            .Arguments
            .FirstOrDefault(a => a.ToString().StartsWith("\""))?
            .ToString()?
            .Trim('"') ?? "type";
        
        var jsonDerivedTypeAttrs = "";
        foreach (var childNode in targetRecord.ChildNodes())
        {
            if (childNode is RecordDeclarationSyntax rds)
            {
                var nameAttribute = rds.FindAttribute("EnumerationItemName");
                var name = rds.Identifier.ToString();
                if (nameAttribute is not null)
                    name = nameAttribute.ArgumentList?.Arguments.FirstOrDefault()?.ToString()?.Trim('"') ?? name;

                //jsonDerivedTypeAttrs += $"\n[JsonDerivedType(typeof({rds.Identifier}), \"{name}\")]";
            }
        }

        var sourceText = SourceText.From(@$"
using System.Text.Json.Serialization;
[JsonPolymorphic(TypeDiscriminatorPropertyName = ""{discriminatorPropertyName}"")]{jsonDerivedTypeAttrs}
public abstract partial record {targetRecord.Identifier}
{{
}}
", Encoding.UTF8);
        
        context.AddSource($"{targetRecord.Identifier}Enumeration.g.cs", sourceText);
    }
}