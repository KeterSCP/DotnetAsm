import SymbolKind from "./SymbolKind";

type RoslynCompletionResult = {
  symbolKind: SymbolKind;
  displayText: string;
  description?: string;
};

export default RoslynCompletionResult;
