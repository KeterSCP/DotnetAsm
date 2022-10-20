namespace DotnetAsm.Core.Models;

public class AsmGenerationResponse
{
    public required string Asm { get; init; }
    public List<string> AsmSummary { get; init; }
    public string? Errors { get; init; }
}