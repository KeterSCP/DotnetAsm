import { MarkerSeverity } from "monaco-editor";

export default class CodeCheckResult {
  id!: string;
  keyword!: string;
  message!: string;
  offsetFrom!: number;
  offsetTo!: number;
  severity!: MarkerSeverity;
  severityNumeric!: number;
}
