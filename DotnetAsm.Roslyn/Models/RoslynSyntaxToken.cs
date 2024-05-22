namespace DotnetAsm.Roslyn.Models;

public class RoslynSyntaxToken
{
    public required string Text { get; init; }
    public required RoslynSyntaxTokenType TokenType { get; init; }

    public static RoslynSyntaxToken Space { get; } = new()
    {
        Text = " ",
        TokenType = RoslynSyntaxTokenType.Text
    };

    public static RoslynSyntaxToken TextToken(string text) => new()
    {
        Text = text,
        TokenType = RoslynSyntaxTokenType.Text
    };

    public static RoslynSyntaxToken KeywordToken(string text) => new()
    {
        Text = text,
        TokenType = RoslynSyntaxTokenType.Keyword
    };

    public static RoslynSyntaxToken FunctionToken(string text) => new()
    {
        Text = text,
        TokenType = RoslynSyntaxTokenType.Function
    };

    public static RoslynSyntaxToken LiteralToken(string text) => new()
    {
        Text = text,
        TokenType = RoslynSyntaxTokenType.Literal
    };
}
