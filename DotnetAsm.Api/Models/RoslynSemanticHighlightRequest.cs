namespace DotnetAsm.Api.Models;

public class RoslynSemanticHighlightRequest
{
    public required Guid ProjectId { get; init; }
    public required string Code { get; init; }
}
