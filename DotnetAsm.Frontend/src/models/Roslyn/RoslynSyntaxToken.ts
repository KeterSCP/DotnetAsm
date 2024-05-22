import RoslynSyntaxTokenType from "./RoslynSyntaxTokenType";

type RoslynSyntaxToken = {
  text: string;
  tokenType: RoslynSyntaxTokenType;
};

export default RoslynSyntaxToken;

export function formatTokenToHtml(token: RoslynSyntaxToken): string {
  switch (token.tokenType) {
    case RoslynSyntaxTokenType.Keyword:
      return `<span style="color:#569CD6;">${token.text}</span>`;
    case RoslynSyntaxTokenType.Class:
      return `<span style="color:#4EC9B0;">${token.text}</span>`;
    case RoslynSyntaxTokenType.Struct:
      return `<span style="color:#86C691;">${token.text}</span>`;
    case RoslynSyntaxTokenType.Function:
      return `<span style="color:#DCDCAA;">${token.text}</span>`;
    case RoslynSyntaxTokenType.Identifier:
      return `<span>${token.text}</span>`;
    default:
      return token.text;
  }
}
