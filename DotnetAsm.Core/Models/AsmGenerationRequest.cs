namespace DotnetAsm.Core.Models;

public class AsmGenerationRequest
{
    public required string CsharpCode { get; init; }
    public string? MethodName { get; init; }
    public required bool UsePgo { get; init; }
    public required bool UseTieredCompilation { get; init; }
    public required bool UseReadyToRun { get; init; }
    public TargetFramework TargetFramework { get; init; }
}