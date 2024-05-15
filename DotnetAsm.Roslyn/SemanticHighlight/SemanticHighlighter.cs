using DotnetAsm.Roslyn.SemanticHighlight.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;

namespace DotnetAsm.Roslyn.SemanticHighlight;

internal static class SemanticHighlighter
{
    private static readonly Dictionary<string, SemanticHighlightClassification> ClassificationMap =
        new()
        {
            [ClassificationTypeNames.Comment] = SemanticHighlightClassification.Comment,
            [ClassificationTypeNames.ExcludedCode] = SemanticHighlightClassification.ExcludedCode,
            [ClassificationTypeNames.Identifier] = SemanticHighlightClassification.Identifier,
            [ClassificationTypeNames.Keyword] = SemanticHighlightClassification.Keyword,
            [ClassificationTypeNames.ControlKeyword] = SemanticHighlightClassification.ControlKeyword,
            [ClassificationTypeNames.NumericLiteral] = SemanticHighlightClassification.NumericLiteral,
            [ClassificationTypeNames.Operator] = SemanticHighlightClassification.Operator,
            [ClassificationTypeNames.OperatorOverloaded] = SemanticHighlightClassification.OperatorOverloaded,
            [ClassificationTypeNames.PreprocessorKeyword] = SemanticHighlightClassification.PreprocessorKeyword,
            [ClassificationTypeNames.StringLiteral] = SemanticHighlightClassification.StringLiteral,
            [ClassificationTypeNames.WhiteSpace] = SemanticHighlightClassification.WhiteSpace,
            [ClassificationTypeNames.Text] = SemanticHighlightClassification.Text,
            [ClassificationTypeNames.StaticSymbol] = SemanticHighlightClassification.StaticSymbol,
            [ClassificationTypeNames.PreprocessorText] = SemanticHighlightClassification.PreprocessorText,
            [ClassificationTypeNames.Punctuation] = SemanticHighlightClassification.Punctuation,
            [ClassificationTypeNames.VerbatimStringLiteral] = SemanticHighlightClassification.VerbatimStringLiteral,
            [ClassificationTypeNames.StringEscapeCharacter] = SemanticHighlightClassification.StringEscapeCharacter,
            [ClassificationTypeNames.ClassName] = SemanticHighlightClassification.ClassName,
            [ClassificationTypeNames.RecordClassName] = SemanticHighlightClassification.ClassName,
            [ClassificationTypeNames.DelegateName] = SemanticHighlightClassification.DelegateName,
            [ClassificationTypeNames.EnumName] = SemanticHighlightClassification.EnumName,
            [ClassificationTypeNames.InterfaceName] = SemanticHighlightClassification.InterfaceName,
            [ClassificationTypeNames.ModuleName] = SemanticHighlightClassification.ModuleName,
            [ClassificationTypeNames.StructName] = SemanticHighlightClassification.StructName,
            [ClassificationTypeNames.RecordStructName] = SemanticHighlightClassification.StructName,
            [ClassificationTypeNames.TypeParameterName] = SemanticHighlightClassification.TypeParameterName,
            [ClassificationTypeNames.FieldName] = SemanticHighlightClassification.FieldName,
            [ClassificationTypeNames.EnumMemberName] = SemanticHighlightClassification.EnumMemberName,
            [ClassificationTypeNames.ConstantName] = SemanticHighlightClassification.ConstantName,
            [ClassificationTypeNames.LocalName] = SemanticHighlightClassification.LocalName,
            [ClassificationTypeNames.ParameterName] = SemanticHighlightClassification.ParameterName,
            [ClassificationTypeNames.MethodName] = SemanticHighlightClassification.MethodName,
            [ClassificationTypeNames.ExtensionMethodName] = SemanticHighlightClassification.ExtensionMethodName,
            [ClassificationTypeNames.PropertyName] = SemanticHighlightClassification.PropertyName,
            [ClassificationTypeNames.EventName] = SemanticHighlightClassification.EventName,
            [ClassificationTypeNames.NamespaceName] = SemanticHighlightClassification.NamespaceName,
            [ClassificationTypeNames.LabelName] = SemanticHighlightClassification.LabelName,
            [ClassificationTypeNames.XmlDocCommentAttributeName] = SemanticHighlightClassification.XmlDocCommentAttributeName,
            [ClassificationTypeNames.XmlDocCommentAttributeQuotes] = SemanticHighlightClassification.XmlDocCommentAttributeQuotes,
            [ClassificationTypeNames.XmlDocCommentAttributeValue] = SemanticHighlightClassification.XmlDocCommentAttributeValue,
            [ClassificationTypeNames.XmlDocCommentCDataSection] = SemanticHighlightClassification.XmlDocCommentCDataSection,
            [ClassificationTypeNames.XmlDocCommentComment] = SemanticHighlightClassification.XmlDocCommentComment,
            [ClassificationTypeNames.XmlDocCommentDelimiter] = SemanticHighlightClassification.XmlDocCommentDelimiter,
            [ClassificationTypeNames.XmlDocCommentEntityReference] = SemanticHighlightClassification.XmlDocCommentEntityReference,
            [ClassificationTypeNames.XmlDocCommentName] = SemanticHighlightClassification.XmlDocCommentName,
            [ClassificationTypeNames.XmlDocCommentProcessingInstruction] = SemanticHighlightClassification.XmlDocCommentProcessingInstruction,
            [ClassificationTypeNames.XmlDocCommentText] = SemanticHighlightClassification.XmlDocCommentText,
            [ClassificationTypeNames.XmlLiteralAttributeName] = SemanticHighlightClassification.XmlLiteralAttributeName,
            [ClassificationTypeNames.XmlLiteralAttributeQuotes] = SemanticHighlightClassification.XmlLiteralAttributeQuotes,
            [ClassificationTypeNames.XmlLiteralAttributeValue] = SemanticHighlightClassification.XmlLiteralAttributeValue,
            [ClassificationTypeNames.XmlLiteralCDataSection] = SemanticHighlightClassification.XmlLiteralCDataSection,
            [ClassificationTypeNames.XmlLiteralComment] = SemanticHighlightClassification.XmlLiteralComment,
            [ClassificationTypeNames.XmlLiteralDelimiter] = SemanticHighlightClassification.XmlLiteralDelimiter,
            [ClassificationTypeNames.XmlLiteralEmbeddedExpression] = SemanticHighlightClassification.XmlLiteralEmbeddedExpression,
            [ClassificationTypeNames.XmlLiteralEntityReference] = SemanticHighlightClassification.XmlLiteralEntityReference,
            [ClassificationTypeNames.XmlLiteralName] = SemanticHighlightClassification.XmlLiteralName,
            [ClassificationTypeNames.XmlLiteralProcessingInstruction] = SemanticHighlightClassification.XmlLiteralProcessingInstruction,
            [ClassificationTypeNames.XmlLiteralText] = SemanticHighlightClassification.XmlLiteralText,
            [ClassificationTypeNames.RegexComment] = SemanticHighlightClassification.RegexComment,
            [ClassificationTypeNames.RegexCharacterClass] = SemanticHighlightClassification.RegexCharacterClass,
            [ClassificationTypeNames.RegexAnchor] = SemanticHighlightClassification.RegexAnchor,
            [ClassificationTypeNames.RegexQuantifier] = SemanticHighlightClassification.RegexQuantifier,
            [ClassificationTypeNames.RegexGrouping] = SemanticHighlightClassification.RegexGrouping,
            [ClassificationTypeNames.RegexAlternation] = SemanticHighlightClassification.RegexAlternation,
            [ClassificationTypeNames.RegexText] = SemanticHighlightClassification.RegexText,
            [ClassificationTypeNames.RegexSelfEscapedCharacter] = SemanticHighlightClassification.RegexSelfEscapedCharacter,
            [ClassificationTypeNames.RegexOtherEscape] = SemanticHighlightClassification.RegexOtherEscape,
        };

    public static async Task<List<SemanticHighlightSpan>> GetHighlightSpansAsync(Document document, CancellationToken ct = default)
    {
        var text = await document.GetTextAsync(ct);
        var textSpan = new TextSpan(0, text.Length);

        var classifiedSpans = await Classifier.GetClassifiedSpansAsync(document, textSpan, ct);

        var results = classifiedSpans
            .GroupBy(s => s.TextSpan.ToString())
            .Select(g => CreateSemanticSpan(g, text.Lines))
            .ToList();

        return results;
    }

    private static  SemanticHighlightSpan CreateSemanticSpan(IEnumerable<ClassifiedSpan> results, TextLineCollection lines)
    {
        var resultsList = results.ToList();

        var additiveResults = resultsList.Where(result => ClassificationTypeNames.AdditiveTypeNames.Contains(result.ClassificationType));

        var span = resultsList.Except(additiveResults).Single();

        var linePosition = lines.GetLinePositionSpan(span.TextSpan);

        ClassificationMap.TryGetValue(span.ClassificationType, out var type);

        return new SemanticHighlightSpan
        {
            StartLine = linePosition.Start.Line,
            EndLine = linePosition.End.Line,
            StartColumn = linePosition.Start.Character,
            EndColumn = linePosition.End.Character,
            ClassificationType = type
        };
    }
}
