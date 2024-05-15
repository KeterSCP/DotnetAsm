using DotnetAsm.Roslyn.Models;

using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Extensions;

internal static class ITypeSymbolExtensions
{
    public static bool IsKnownAlias(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.SpecialType
            is SpecialType.System_SByte or SpecialType.System_Byte
            or SpecialType.System_Int16 or SpecialType.System_UInt16
            or SpecialType.System_Int32 or SpecialType.System_UInt32
            or SpecialType.System_Int64 or SpecialType.System_UInt64
            or SpecialType.System_Single or SpecialType.System_Double
            or SpecialType.System_Boolean
            or SpecialType.System_Char or SpecialType.System_String
            or SpecialType.System_Object
            or SpecialType.System_IntPtr or SpecialType.System_UIntPtr
            or SpecialType.System_Void;
    }

    public static RoslynSyntaxTokenType GetTokenType(this ITypeSymbol typeSymbol, bool expandKnownAliasName = false)
    {
        if (typeSymbol.IsKnownAlias() && !expandKnownAliasName)
        {
            return RoslynSyntaxTokenType.Keyword;
        }

        return typeSymbol.TypeKind switch
        {
            TypeKind.Class => RoslynSyntaxTokenType.Class,
            TypeKind.Struct => RoslynSyntaxTokenType.Struct,
            TypeKind.Enum => RoslynSyntaxTokenType.Struct,
            _ => RoslynSyntaxTokenType.Text
        };
    }

    public static string GetDisplayText(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsKnownAlias())
        {
            return typeSymbol.ToDisplayString();
        }

        return typeSymbol.MetadataName;
    }
}
