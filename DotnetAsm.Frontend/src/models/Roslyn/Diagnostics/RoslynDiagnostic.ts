import RoslynDiagnosticSeverity from "./RoslynDiagnosticSeverity";

type RoslynDiagnostic = {
  id: string;
  message: string;
  offsetFrom: number;
  offsetTo: number;
  severity: RoslynDiagnosticSeverity;
};

export default RoslynDiagnostic;
