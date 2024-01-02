using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MagpiEnums.SourceGeneration.Extensions;

public static class RecordDeclarationSyntaxExtensions
{
    public static bool HasAttribute(this RecordDeclarationSyntax self, string attributeName)
    {
        return self.AttributeLists
            .SelectMany(a => a.Attributes)
            .Any(a => a.Name.NormalizeWhitespace().ToString() == attributeName);
    }

    public static AttributeSyntax? FindAttribute(this RecordDeclarationSyntax self, string attributeName)
    {
        return self.AttributeLists
            .SelectMany(a => a.Attributes)
            .FirstOrDefault(a => a.Name.NormalizeWhitespace().ToString() == attributeName);
    }
}