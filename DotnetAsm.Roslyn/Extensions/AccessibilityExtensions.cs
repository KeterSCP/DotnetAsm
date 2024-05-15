using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Extensions;

internal static class AccessibilityExtensions
{
    public static string ToDisplayString(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => string.Empty
        };
    }
}
