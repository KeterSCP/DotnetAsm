import SemanticHighlightClassification from "./SemanticHighlightClassification";

type SemanticHighlightSpan = {
  startLine: number;
  startColumn: number;
  endLine: number;
  endColumn: number;
  classificationType: SemanticHighlightClassification;
};

export default SemanticHighlightSpan;
