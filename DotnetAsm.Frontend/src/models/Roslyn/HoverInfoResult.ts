import RoslynSyntaxToken from "./RoslynSyntaxToken";

export default class HoverInfoResult {
  tokens!: RoslynSyntaxToken[];
  offsetFrom!: number;
  offsetTo!: number;
}
