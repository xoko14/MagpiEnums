using System.Text;
using MagpiEnums.SourceGeneration.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagpiEnums.SourceGeneration;

[Generator]
public class EnumerationGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var recordProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName("MagpiEnums.EnumerationAttribute",
                (node, _) => node is RecordDeclarationSyntax,
                GetInfo
            );
        context.RegisterSourceOutput(recordProvider, Generate);
    }

    private (RecordDeclarationSyntax Node, string? Namespace) GetInfo(GeneratorAttributeSyntaxContext ctx, CancellationToken _)
    {
        var type = (INamedTypeSymbol) ctx.TargetSymbol;
        return (
            (RecordDeclarationSyntax)ctx.TargetNode,
            type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToString()
        );
    }

    public void Generate(SourceProductionContext context, (RecordDeclarationSyntax TargetRecord, string? Namespace) payload)
    {
        var (targetRecord, targetNamespace) = payload;

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

                jsonDerivedTypeAttrs += $"\n[JsonDerivedType(typeof({rds.Identifier}), \"{name}\")]";
            }
        }

        var sourceText = SourceText.From(@$"
{(targetNamespace is null? "": $"namespace {targetNamespace};")}
using System.Text.Json.Serialization;
[JsonPolymorphic(TypeDiscriminatorPropertyName = ""{discriminatorPropertyName}"")]{jsonDerivedTypeAttrs}
public abstract partial record {targetRecord.Identifier}
{{
}}
", Encoding.UTF8);
        
        context.AddSource($"{targetRecord.Identifier}Enumeration.g.cs", sourceText);
    }

}