import * as monaco from "monaco-editor";

/* eslint-disable no-unused-vars */
enum RoslynDiagnosticSeverity {
  Info = 1,
  Warning = 2,
  Error = 3,
}

export default RoslynDiagnosticSeverity;

export function mapToMonacoSeverity(severity: RoslynDiagnosticSeverity): monaco.MarkerSeverity {
  switch (severity) {
    case RoslynDiagnosticSeverity.Info:
      return monaco.MarkerSeverity.Info;
    case RoslynDiagnosticSeverity.Warning:
      return monaco.MarkerSeverity.Warning;
    case RoslynDiagnosticSeverity.Error:
      return monaco.MarkerSeverity.Error;
  }
}
