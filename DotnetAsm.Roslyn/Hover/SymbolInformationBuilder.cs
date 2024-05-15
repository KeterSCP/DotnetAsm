using System.Diagnostics;

using DotnetAsm.Roslyn.Extensions;
using DotnetAsm.Roslyn.Models;

using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Hover;

internal static class SymbolInformationBuilder
{
    public static IReadOnlyList<RoslynSyntaxToken> BuildMethodSymbolInformation(IMethodSymbol methodSymbol)
    {
        var tokens = new List<RoslynSyntaxToken>();

        if (methodSymbol.MethodKind == MethodKind.LocalFunction)
        {
            tokens.Add(RoslynSyntaxToken.TextToken("local function: "));
        }

        tokens.AddRange(methodSymbol.GetModifiersTokens());
        tokens.Add(RoslynSyntaxToken.Space);

        tokens.Add(new RoslynSyntaxToken
        {
            Text = methodSymbol.ReturnType.GetDisplayText(),
            TokenType = methodSymbol.ReturnType.GetTokenType()
        });
        tokens.Add(RoslynSyntaxToken.Space);

        tokens.Add(RoslynSyntaxToken.FunctionToken(methodSymbol.Name));
        tokens.Add(RoslynSyntaxToken.TextToken("("));

        var parameters = methodSymbol.Parameters;

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            tokens.Add(new RoslynSyntaxToken
            {
                Text = parameter.Type.GetDisplayText(),
                TokenType = parameter.Type.GetTokenType()
            });

            tokens.Add(RoslynSyntaxToken.Space);

            tokens.Add(RoslynSyntaxToken.TextToken(parameter.Name));

            if (i < parameters.Length - 1)
            {
                tokens.Add(RoslynSyntaxToken.TextToken(","));
                tokens.Add(RoslynSyntaxToken.Space);
            }
        }

        tokens.Add(RoslynSyntaxToken.TextToken(")"));

        return tokens;
    }

    public static IReadOnlyList<RoslynSyntaxToken> BuildTypeSymbolInformation(ITypeSymbol typeSymbol)
    {
        var tokens = new List<RoslynSyntaxToken>();

        tokens.AddRange(typeSymbol.GetModifiersTokens());
        tokens.Add(RoslynSyntaxToken.Space);

        string? typeKindText = typeSymbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            TypeKind.Enum => "enum",
            TypeKind.Delegate => "delegate",
            _ => null
        };

        if (typeKindText is not null)
        {
            tokens.Add(RoslynSyntaxToken.KeywordToken(typeKindText));
            tokens.Add(RoslynSyntaxToken.Space);
        }

        tokens.Add(new RoslynSyntaxToken
        {
            Text = typeSymbol.MetadataName,
            TokenType = typeSymbol.GetTokenType(expandKnownAliasName: true)
        });

        return tokens;
    }

    public static IReadOnlyList<RoslynSyntaxToken> BuildLocalSymbolInformation(ILocalSymbol localSymbol)
    {
        return
        [
            RoslynSyntaxToken.TextToken("local variable: "),
            new RoslynSyntaxToken
            {
                Text = localSymbol.Type.GetDisplayText(),
                TokenType = localSymbol.Type.GetTokenType()
            },
            RoslynSyntaxToken.Space,
            RoslynSyntaxToken.TextToken(localSymbol.Name)
        ];
    }

    public static IReadOnlyList<RoslynSyntaxToken> BuildParameterSymbolInformation(IParameterSymbol parameterSymbol)
    {
        return
        [
            RoslynSyntaxToken.TextToken("parameter: "),
            new RoslynSyntaxToken
            {
                Text = parameterSymbol.Type.GetDisplayText(),
                TokenType = parameterSymbol.Type.GetTokenType()
            },
            RoslynSyntaxToken.Space,
            RoslynSyntaxToken.TextToken(parameterSymbol.Name)
        ];
    }

    public static IReadOnlyList<RoslynSyntaxToken> BuildFieldSymbolInformation(IFieldSymbol fieldSymbol)
    {
        if (fieldSymbol.Type.TypeKind is TypeKind.Enum)
        {
            Debug.Assert(fieldSymbol.HasConstantValue);
            Debug.Assert(fieldSymbol.ConstantValue is not null);

            return
            [
                RoslynSyntaxToken.TextToken($"{fieldSymbol.MetadataName} = "),
                RoslynSyntaxToken.LiteralToken(fieldSymbol.ConstantValue.ToString()!)
            ];
        }

        var tokens = new List<RoslynSyntaxToken>();

        tokens.AddRange(fieldSymbol.GetModifiersTokens());
        tokens.Add(RoslynSyntaxToken.Space);
        tokens.Add(new RoslynSyntaxToken
        {
            Text = fieldSymbol.Type.GetDisplayText(),
            TokenType = fieldSymbol.Type.GetTokenType()
        });

        tokens.Add(RoslynSyntaxToken.Space);
        tokens.Add(RoslynSyntaxToken.TextToken(fieldSymbol.Name));

        if (fieldSymbol is { HasConstantValue: true, ConstantValue: not null })
        {
            tokens.Add(RoslynSyntaxToken.Space);
            tokens.Add(RoslynSyntaxToken.TextToken("="));
            tokens.Add(RoslynSyntaxToken.Space);
            tokens.Add(RoslynSyntaxToken.LiteralToken(fieldSymbol.ConstantValue.ToString()!));
        }

        return tokens;
    }
}
