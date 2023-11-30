using MagpiEnums.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MagpiEnums.SourceGeneration;

public class EnumerationSyntaxReciever: ISyntaxReceiver
{
    public RecordDeclarationSyntax? TargetRecord { get; private set; }
    
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is RecordDeclarationSyntax rds && rds.HasAttribute("Enumeration")) 
            TargetRecord = rds;
    }
}