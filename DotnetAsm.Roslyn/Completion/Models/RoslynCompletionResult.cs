using JetBrains.Annotations;

using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Completion.Models;

[PublicAPI]
public class RoslynCompletionResult
{
    public required SymbolKind SymbolKind { get; init; }
    public required string DisplayText { get; init; }
    public required string? Description { get; init; }
}
