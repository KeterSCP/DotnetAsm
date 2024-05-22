using DotnetAsm.Roslyn.Models;

using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Extensions;

internal static class ISymbolExtensions
{
    public static IReadOnlyList<RoslynSyntaxToken> GetModifiersTokens(this ISymbol symbol)
    {
        var result = new List<RoslynSyntaxToken>
        {
            RoslynSyntaxToken.KeywordToken(symbol.DeclaredAccessibility.ToDisplayString())
        };

        if (symbol is IPropertySymbol { IsRequired: true })
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("required"));
        }

        if (symbol.IsStatic)
        {
            result.Add(RoslynSyntaxToken.Space);

            if (symbol is IFieldSymbol { IsConst: true })
            {
                result.Add(RoslynSyntaxToken.KeywordToken("const"));
            }
            else
            {
                result.Add(RoslynSyntaxToken.KeywordToken("static"));
            }
        }

        if (symbol.IsAbstract)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("abstract"));
        }

        if (symbol.IsVirtual)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("virtual"));
        }

        if (symbol.IsOverride)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("override"));
        }

        if (symbol.IsSealed)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("sealed"));
        }

        if (symbol.IsExtern)
        {
            result.Add(RoslynSyntaxToken.Space);
            result.Add(RoslynSyntaxToken.KeywordToken("extern"));
        }

        return result;
    }
}
