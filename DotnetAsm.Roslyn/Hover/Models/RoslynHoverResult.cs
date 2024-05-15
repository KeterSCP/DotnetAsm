using DotnetAsm.Roslyn.Models;

using JetBrains.Annotations;

namespace DotnetAsm.Roslyn.Hover.Models;

[PublicAPI]
public class RoslynHoverResult
{
    public required IReadOnlyList<RoslynSyntaxToken> Tokens { get; init; }
    public required int OffsetFrom { get; init; }
    public required int OffsetTo { get; init; }
}
