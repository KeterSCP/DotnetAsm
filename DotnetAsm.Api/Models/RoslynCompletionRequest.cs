using JetBrains.Annotations;

namespace DotnetAsm.Api.Models;

[PublicAPI]
public class RoslynCompletionRequest
{
    public required Guid ProjectId { get; init; }
    public required string Code { get; init; }
    public required int Position { get; init; }
}
