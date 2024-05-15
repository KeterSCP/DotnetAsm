import type monacoTypes from "monaco-editor/esm/vs/editor/editor.api";

import * as monaco from "monaco-editor";
import RoslynCompletionResult from "src/models/Roslyn/RoslynCompletionResult";
import SignatureHelpResult from "src/models/Roslyn/SignatureHelpResult";
import HoverInfoResult from "src/models/Roslyn/HoverInfoResult";
import CodeCheckResult from "src/models/Roslyn/CodeCheckResult";
import { mapToMonacoCompletionItemKind } from "src/models/Roslyn/SymbolKind";
import { formatTokenToHtml } from "src/models/Roslyn/RoslynSyntaxToken";
import { languageName } from "./csharp-semantic-lang";
import SemanticHighlightSpan from "src/models/Roslyn/SemanticHighlight/SemanticHighlightSpan";
import CSharpTokenType from "./CSharpTokenType";
import { mapToMonacoTokenNumber } from "src/models/Roslyn/SemanticHighlight/SemanticHighlightClassification";

export default function registerCSharpLanguageProvider(): void {
  registerCompletions();
  // registerSignatureCompletions();
  registerHoverCompletions();
  registerSemanticTokensProvider();
  // registerCodeChecks();
}

function registerSemanticTokensProvider() {
  monaco.languages.registerDocumentSemanticTokensProvider(languageName, {
    getLegend: () => {
      return {
        tokenTypes: Object.values(CSharpTokenType).filter((v) => isNaN(Number(v))) as string[],
        tokenModifiers: [],
      };
    },

    provideDocumentSemanticTokens: async (model) => {
      const highlights = await sendPostRequest<SemanticHighlightSpan[]>("/api/roslyn/semantic-highlight", {
        Code: model.getValue(),
        ProjectId: "29BFD5DD-B9FC-45E1-A5C3-DC3218F3779E",
      });

      const data = [] as number[];

      let prevLine = 0;
      let prevChar = 0;

      for (const highlight of highlights) {
        const line = highlight.endLine;
        const length = highlight.endColumn - highlight.startColumn;
        const tokenType = mapToMonacoTokenNumber(highlight.classificationType);

        const deltaLine = line - prevLine;
        const deltaChar = deltaLine === 0 ? highlight.startColumn - prevChar : highlight.startColumn;

        prevLine = line;
        prevChar = highlight.startColumn;

        data.push(
          deltaLine,
          deltaChar,
          length,
          tokenType,
          0 // modifiers
        );
      }

      return {
        data: new Uint32Array(data),
        resultId: undefined,
      };
    },

    releaseDocumentSemanticTokens: () => {
      // Nothing to release
      return;
    },
  });
}

function registerCompletions() {
  monaco.languages.registerCompletionItemProvider(languageName, {
    triggerCharacters: ["."],
    provideCompletionItems: async (model, position) => {
      const suggestions: monacoTypes.languages.CompletionItem[] = [];

      const generationResponse = await sendPostRequest<RoslynCompletionResult[]>("/api/roslyn/complete", {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        // TODO: generate this
        ProjectId: "29BFD5DD-B9FC-45E1-A5C3-DC3218F3779E",
      });

      for (const elem of generationResponse) {
        const word = model.getWordUntilPosition(position);

        suggestions.push({
          label: {
            label: elem.displayText,
          },
          kind: mapToMonacoCompletionItemKind(elem.symbolKind),
          insertText: elem.displayText,
          documentation: elem.description,
          range: {
            startLineNumber: position.lineNumber,
            endLineNumber: position.lineNumber,
            startColumn: word.startColumn,
            endColumn: word.endColumn,
          },
        });
      }

      return { suggestions: suggestions };
    },
  });
}

function registerSignatureCompletions() {
  monaco.languages.registerSignatureHelpProvider(languageName, {
    signatureHelpTriggerCharacters: ["("],
    signatureHelpRetriggerCharacters: [","],
    provideSignatureHelp: async (model, position) => {
      const signatures = [];

      const responseSignatures = await sendPostRequest<SignatureHelpResult>("/api/roslyn/signature", {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        Assemblies: [],
      });

      for (const signature of responseSignatures.signatures) {
        const params = [];
        for (const param of signature.parameters) {
          params.push({
            label: param.label,
            documentation: param.documentation ?? "",
          });
        }
        signatures.push({
          label: signature.label,
          documentation: signature.documentation ?? "",
          parameters: params,
        });
      }

      return {
        value: {
          signatures: signatures,
          activeParameter: responseSignatures.activeParameter,
          activeSignature: responseSignatures.activeSignature,
        },
        dispose: () => {
          // Nothing to dispose
        },
      };
    },
  });
}

function registerHoverCompletions() {
  monaco.languages.registerHoverProvider(languageName, {
    provideHover: async function (model, position) {
      const hoverInfoResponse = await sendPostRequest<HoverInfoResult>("/api/roslyn/hover", {
        // TODO: generate this
        ProjectId: "29BFD5DD-B9FC-45E1-A5C3-DC3218F3779E",
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
      });

      const posStart = model.getPositionAt(hoverInfoResponse.offsetFrom);
      const posEnd = model.getPositionAt(hoverInfoResponse.offsetTo);

      const information = hoverInfoResponse.tokens.map((token) => formatTokenToHtml(token)).join("");

      return {
        range: {
          startLineNumber: posStart.lineNumber,
          startColumn: posStart.column,
          endLineNumber: posEnd.lineNumber,
          endColumn: posEnd.column,
        },
        contents: [{ value: information, isTrusted: true, supportHtml: true }],
      };
    },
  });
}

function registerCodeChecks() {
  monaco.editor.onDidCreateModel(function (model) {
    async function validate() {
      const codeCheckResponse = await sendPostRequest<CodeCheckResult[]>("/api/roslyn/code-check", {
        Code: model.getValue(),
        Assemblies: [],
      });

      const markers = [];

      for (const elem of codeCheckResponse) {
        const posStart = model.getPositionAt(elem.offsetFrom);
        const posEnd = model.getPositionAt(elem.offsetTo);
        markers.push({
          severity: elem.severity,
          startLineNumber: posStart.lineNumber,
          startColumn: posStart.column,
          endLineNumber: posEnd.lineNumber,
          endColumn: posEnd.column,
          message: elem.message,
          code: elem.id,
        });
      }

      monaco.editor.setModelMarkers(model, languageName, markers);
    }

    let handle: NodeJS.Timeout | null = null;
    model.onDidChangeContent(() => {
      monaco.editor.setModelMarkers(model, languageName, []);
      if (handle) {
        clearTimeout(handle);
      }
      handle = setTimeout(() => validate(), 500);
    });
    validate();
  });
}

async function sendPostRequest<T>(url: string, data: unknown): Promise<T> {
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  try {
    const json = await response.json();

    const result = json as T;
    return result;
  } catch (e) {
    console.error(e);
    throw e;
  }
}
