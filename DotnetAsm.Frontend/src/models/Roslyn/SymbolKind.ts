/* eslint-disable no-unused-vars */
import * as monaco from "monaco-editor";

const enum SymbolKind {
  Alias = 0,
  ArrayType = 1,
  Assembly = 2,
  DynamicType = 3,
  ErrorType = 4,
  Event = 5,
  Field = 6,
  Label = 7,
  Local = 8,
  Method = 9,
  NetModule = 10,
  NamedType = 11,
  Namespace = 12,
  Parameter = 13,
  PointerType = 14,
  Property = 15,
  RangeVariable = 16,
  TypeParameter = 17,
  Preprocessing = 18,
  Discard = 19,
  FunctionPointerType = 20,
}

export default SymbolKind;

export function mapToMonacoCompletionItemKind(symbolKind: SymbolKind): monaco.languages.CompletionItemKind {
  switch (symbolKind) {
    case SymbolKind.Alias:
      return monaco.languages.CompletionItemKind.Module;
    case SymbolKind.ArrayType:
      return monaco.languages.CompletionItemKind.Class;
    case SymbolKind.Assembly:
      return monaco.languages.CompletionItemKind.Module;
    case SymbolKind.DynamicType:
      return monaco.languages.CompletionItemKind.Class;
    case SymbolKind.ErrorType:
      return monaco.languages.CompletionItemKind.Class;
    case SymbolKind.Event:
      return monaco.languages.CompletionItemKind.Event;
    case SymbolKind.Field:
      return monaco.languages.CompletionItemKind.Field;
    case SymbolKind.Label:
      return monaco.languages.CompletionItemKind.Reference;
    case SymbolKind.Local:
      return monaco.languages.CompletionItemKind.Variable;
    case SymbolKind.Method:
      return monaco.languages.CompletionItemKind.Method;
    case SymbolKind.NetModule:
      return monaco.languages.CompletionItemKind.Module;
    case SymbolKind.NamedType:
      return monaco.languages.CompletionItemKind.Class;
    case SymbolKind.Namespace:
      return monaco.languages.CompletionItemKind.Module;
    case SymbolKind.Parameter:
      return monaco.languages.CompletionItemKind.Variable;
    case SymbolKind.PointerType:
      return monaco.languages.CompletionItemKind.Class;
    case SymbolKind.Property:
      return monaco.languages.CompletionItemKind.Property;
    case SymbolKind.RangeVariable:
      return monaco.languages.CompletionItemKind.Variable;
    case SymbolKind.TypeParameter:
      return monaco.languages.CompletionItemKind.TypeParameter;
    case SymbolKind.Preprocessing:
      return monaco.languages.CompletionItemKind.Module;
    case SymbolKind.Discard:
      return monaco.languages.CompletionItemKind.Variable;
    case SymbolKind.FunctionPointerType:
      return monaco.languages.CompletionItemKind.Function;
    default:
      return monaco.languages.CompletionItemKind.Text;
  }
}
