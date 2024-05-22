namespace DotnetAsm.Api.Models;

public class RoslynHoverRequest
{
    public required Guid ProjectId { get; init; }
    public required string Code { get; init; }
    public required int Position { get; init; }
}
