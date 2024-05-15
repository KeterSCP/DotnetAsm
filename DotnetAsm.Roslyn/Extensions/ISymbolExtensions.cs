using DotnetAsm.Roslyn.Models;

using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Extensions;

internal static class ISymbolExtensions
{
    public static IReadOnlyList<RoslynSyntaxToken> GetModifiersTokens(this ISymbol methodSymbol)
    {
        var result = new List<RoslynSyntaxToken>
        {
            RoslynSyntaxToken.KeywordToken(methodSymbol.DeclaredAccessibility.ToDisplayString())
        };

        if (methodSymbol.IsStatic)
        {
            result.Add(RoslynSyntaxToken.Space);

            if (methodSymbol is IFieldSymbol { IsConst: true })
            {
                result.Add(RoslynSyntaxToken.KeywordToken("const"));
            }
            else
            {
                result.Add(RoslynSyntaxToken.KeywordToken("static"));
            }
        }

        if (methodSymbol.IsAbstract)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("abstract"));
        }

        if (methodSymbol.IsVirtual)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("virtual"));
        }

        if (methodSymbol.IsOverride)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("override"));
        }

        if (methodSymbol.IsSealed)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("sealed"));
        }

        if (methodSymbol.IsExtern)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("extern"));
        }

        return result;
    }
}
