namespace DotnetAsm.Roslyn.SemanticHighlight.Models;

public class SemanticHighlightSpan
{
    public required int StartLine { get; init; }
    public required int StartColumn { get; init; }
    public required int EndLine { get; init; }
    public required int EndColumn { get; init; }
    public required SemanticHighlightClassification ClassificationType { get; init; }
}
