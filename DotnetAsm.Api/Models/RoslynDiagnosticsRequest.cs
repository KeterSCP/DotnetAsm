namespace DotnetAsm.Api.Models;

public class RoslynDiagnosticsRequest
{
    public required Guid ProjectId { get; init; }
    public required string Code { get; init; }
}
