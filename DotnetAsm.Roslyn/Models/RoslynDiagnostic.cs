using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Models;

public class RoslynDiagnostic
{
    public required string Id { get; init; }
    public required string Message { get; init; }
    public required int OffsetFrom { get; init; }
    public required int OffsetTo { get; init; }
    public required DiagnosticSeverity Severity { get; init; }
}
