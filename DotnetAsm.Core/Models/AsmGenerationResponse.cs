namespace DotnetAsm.Core.Models;

public class AsmGenerationResponse
{
    public required string Asm { get; init; }
    public string? AsmSummary { get; init; }
    public string? Errors { get; init; }
}