/* eslint-disable no-unused-vars */

import CSharpTokenType from "src/monaco/CSharpTokenType";

const enum SemanticHighlightClassification {
  Comment = 0,
  ExcludedCode = 1,
  Identifier = 2,
  Keyword = 3,
  ControlKeyword = 4,
  NumericLiteral = 5,
  Operator = 6,
  OperatorOverloaded = 7,
  PreprocessorKeyword = 8,
  StringLiteral = 9,
  WhiteSpace = 10,
  Text = 11,
  StaticSymbol = 12,
  PreprocessorText = 13,
  Punctuation = 14,
  VerbatimStringLiteral = 15,
  StringEscapeCharacter = 16,
  ClassName = 17,
  DelegateName = 18,
  EnumName = 19,
  InterfaceName = 20,
  ModuleName = 21,
  StructName = 22,
  TypeParameterName = 23,
  FieldName = 24,
  EnumMemberName = 25,
  ConstantName = 26,
  LocalName = 27,
  ParameterName = 28,
  MethodName = 29,
  ExtensionMethodName = 30,
  PropertyName = 31,
  EventName = 32,
  NamespaceName = 33,
  LabelName = 34,
  XmlDocCommentAttributeName = 35,
  XmlDocCommentAttributeQuotes = 36,
  XmlDocCommentAttributeValue = 37,
  XmlDocCommentCDataSection = 38,
  XmlDocCommentComment = 39,
  XmlDocCommentDelimiter = 40,
  XmlDocCommentEntityReference = 41,
  XmlDocCommentName = 42,
  XmlDocCommentProcessingInstruction = 43,
  XmlDocCommentText = 44,
  XmlLiteralAttributeName = 45,
  XmlLiteralAttributeQuotes = 46,
  XmlLiteralAttributeValue = 47,
  XmlLiteralCDataSection = 48,
  XmlLiteralComment = 49,
  XmlLiteralDelimiter = 50,
  XmlLiteralEmbeddedExpression = 51,
  XmlLiteralEntityReference = 52,
  XmlLiteralName = 53,
  XmlLiteralProcessingInstruction = 54,
  XmlLiteralText = 55,
  RegexComment = 56,
  RegexCharacterClass = 57,
  RegexAnchor = 58,
  RegexQuantifier = 59,
  RegexGrouping = 60,
  RegexAlternation = 61,
  RegexText = 62,
  RegexSelfEscapedCharacter = 63,
  RegexOtherEscape = 64,
}

export default SemanticHighlightClassification;

export function mapToMonacoTokenNumber(classificationType: SemanticHighlightClassification): number {
  switch (classificationType) {
    case SemanticHighlightClassification.Comment:
      return CSharpTokenType.comment;
    case SemanticHighlightClassification.ExcludedCode:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.Identifier:
      return CSharpTokenType.type;
    case SemanticHighlightClassification.Keyword:
      return CSharpTokenType.keyword;
    case SemanticHighlightClassification.ControlKeyword:
      return CSharpTokenType.keyword;
    case SemanticHighlightClassification.NumericLiteral:
      return CSharpTokenType.number;
    case SemanticHighlightClassification.Operator:
      return CSharpTokenType.operator;
    case SemanticHighlightClassification.OperatorOverloaded:
      return CSharpTokenType.operator;
    case SemanticHighlightClassification.PreprocessorKeyword:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.StringLiteral:
      return CSharpTokenType.string;
    case SemanticHighlightClassification.WhiteSpace:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.Text:
      return CSharpTokenType.string;
    case SemanticHighlightClassification.StaticSymbol:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.PreprocessorText:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.Punctuation:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.VerbatimStringLiteral:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.StringEscapeCharacter:
      // TODO:
      return CSharpTokenType.rawText;
    case SemanticHighlightClassification.ClassName:
      return CSharpTokenType.type;
    case SemanticHighlightClassification.DelegateName:
      return CSharpTokenType.function;
    case SemanticHighlightClassification.EnumName:
      return CSharpTokenType.enum;
    case SemanticHighlightClassification.InterfaceName:
      return CSharpTokenType.interface;
    case SemanticHighlightClassification.ModuleName:
      return CSharpTokenType.interface;
    case SemanticHighlightClassification.StructName:
      return CSharpTokenType.type;
    case SemanticHighlightClassification.TypeParameterName:
      return CSharpTokenType.typeParameter;
    case SemanticHighlightClassification.FieldName:
      return CSharpTokenType.property;
    case SemanticHighlightClassification.EnumMemberName:
      return CSharpTokenType.enumMember;
    case SemanticHighlightClassification.ConstantName:
      return CSharpTokenType.property;
    case SemanticHighlightClassification.LocalName:
      return CSharpTokenType.variable;
    case SemanticHighlightClassification.ParameterName:
      return CSharpTokenType.parameter;
    case SemanticHighlightClassification.MethodName:
      return CSharpTokenType.method;
    case SemanticHighlightClassification.ExtensionMethodName:
      return CSharpTokenType.method;
    case SemanticHighlightClassification.PropertyName:
      return CSharpTokenType.property;
    case SemanticHighlightClassification.EventName:
      return CSharpTokenType.event;
    case SemanticHighlightClassification.NamespaceName:
      return CSharpTokenType.namespace;
    case SemanticHighlightClassification.LabelName:
      // TODO:
      return CSharpTokenType.rawText;
    default:
      // TODO:
      return CSharpTokenType.rawText;
  }
}
