import type monacoTypes from "monaco-editor/esm/vs/editor/editor.api";

import * as monaco from "monaco-editor";
import TabCompletionResult from "src/models/Roslyn/TabCompletionResult";
import SignatureHelpResult from "src/models/Roslyn/SignatureHelpResult";
import HoverInfoResult from "src/models/Roslyn/HoverInfoResult";
import CodeCheckResult from "src/models/Roslyn/CodeCheckResult";

export default function registerCSharpLanguageProvider(): void {
    registerCompletions();
    registerSignatureCompletions();
    registerHoverCompletions();
    registerCodeChecks();
}

function registerCompletions() {
    monaco.languages.registerCompletionItemProvider("csharp", {
        triggerCharacters: [".", " "],
        provideCompletionItems: async (model, position) => {
            const suggestions: monacoTypes.languages.CompletionItem[] = [];

            const generationResponse = await sendPostRequest<TabCompletionResult[]>("/api/completion/complete", {
                Code: model.getValue(),
                Position: model.getOffsetAt(position),
                Assemblies: [],
            });

            for (const elem of generationResponse) {
                const word = model.getWordUntilPosition(position);

                suggestions.push({
                    label: {
                        label: elem.suggestion,
                        description: elem.description,
                    },
                    kind: monaco.languages.CompletionItemKind.Function,
                    insertText: elem.suggestion,
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
    monaco.languages.registerSignatureHelpProvider("csharp", {
        signatureHelpTriggerCharacters: ["("],
        signatureHelpRetriggerCharacters: [","],
        provideSignatureHelp: async (model, position) => {
            const signatures = [];

            const responseSignatures = await sendPostRequest<SignatureHelpResult>("/api/completion/signature", {
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
    monaco.languages.registerHoverProvider("csharp", {
        provideHover: async function (model, position) {
            const hoverInfoResponse = await sendPostRequest<HoverInfoResult>("/api/completion/hover", {
                Code: model.getValue(),
                Position: model.getOffsetAt(position),
                Assemblies: [],
            });

            const posStart = model.getPositionAt(hoverInfoResponse.offsetFrom);
            const posEnd = model.getPositionAt(hoverInfoResponse.offsetTo);

            return {
                range: new monaco.Range(posStart.lineNumber, posStart.column, posEnd.lineNumber, posEnd.column),
                contents: [{ value: hoverInfoResponse.information }],
            };
        },
    });
}

function registerCodeChecks() {
    monaco.editor.onDidCreateModel(function (model) {
        async function validate() {
            const codeCheckResponse = await sendPostRequest<CodeCheckResult[]>("/api/completion/code-check", {
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

            monaco.editor.setModelMarkers(model, "csharp", markers);
        }

        let handle: NodeJS.Timeout | null = null;
        model.onDidChangeContent(() => {
            monaco.editor.setModelMarkers(model, "csharp", []);
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

    const responseText = await response.text();
    const result = JSON.parse(responseText) as T;
    return result;
}
