using JetBrains.Annotations;

namespace DotnetAsm.Core.Models;

[PublicAPI]
public class AsmGenerationResponse
{
    public required string Asm { get; init; }
    public IReadOnlyList<string>? AsmSummary { get; init; }
    public string? Errors { get; init; }
}
