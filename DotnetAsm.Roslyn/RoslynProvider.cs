using System.Diagnostics;

using DotnetAsm.Roslyn.Completion.Models;
using DotnetAsm.Roslyn.Extensions;
using DotnetAsm.Roslyn.Hover;
using DotnetAsm.Roslyn.Hover.Models;
using DotnetAsm.Roslyn.Models;
using DotnetAsm.Roslyn.SemanticHighlight;
using DotnetAsm.Roslyn.SemanticHighlight.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetAsm.Roslyn;

public static class RoslynProvider
{
    public static async Task<List<RoslynCompletionResult>> GetCompletionsAsync(
        Guid projectId,
        string sourceCode,
        int position,
        RoslynWorkspaceWrapper workspaceWrapper,
        CancellationToken ct = default)
    {
        var projectWrapper = await workspaceWrapper.GetOrCreateProjectAsync(projectId, ct);

        await projectWrapper.UpdateSourceCodeAndCompileAsync(sourceCode, ct);

        var completionService = CompletionService.GetService(projectWrapper.Document);

        ArgumentNullException.ThrowIfNull(completionService);

        var completionList = await completionService.GetCompletionsAsync(projectWrapper.Document, position, cancellationToken: ct);
        var results = new List<RoslynCompletionResult>(completionList.ItemsList.Count);

        foreach (var completionItem in completionList.ItemsList)
        {
            if (!completionItem.Properties.TryGetValue("SymbolKind", out var symbolKind))
            {
                continue;
            }

            var symbolKindValue = Enum.Parse<SymbolKind>(symbolKind);

            var description = await completionService.GetDescriptionAsync(projectWrapper.Document, completionItem, cancellationToken: ct);
            var completion = new RoslynCompletionResult
            {
                SymbolKind = symbolKindValue,
                DisplayText = completionItem.DisplayText,
                Description = description?.Text,
            };

            results.Add(completion);
        }

        return results;
    }

    public static async Task<RoslynHoverResult> GetHoverAsync(
        Guid projectId,
        string sourceCode,
        int position,
        RoslynWorkspaceWrapper workspaceWrapper,
        CancellationToken ct = default)
    {
        var projectWrapper = await workspaceWrapper.GetOrCreateProjectAsync(projectId, ct);

        await projectWrapper.UpdateSourceCodeAndCompileAsync(sourceCode, ct);

        var syntaxRoot = await projectWrapper.Document.GetSyntaxRootAsync(ct);
        ArgumentNullException.ThrowIfNull(syntaxRoot);
        var syntaxNode = syntaxRoot.FindToken(position).Parent!;

        IReadOnlyList<RoslynSyntaxToken> tokens = [];

        var location = syntaxNode.GetLocation();

        var offsetFrom = location.SourceSpan.Start;
        var offsetTo = location.SourceSpan.End;

        // Check if the position is within the syntax node
        if (position < offsetFrom || position > offsetTo)
        {
            return new RoslynHoverResult
            {
                Tokens = [],
                OffsetFrom = offsetFrom,
                OffsetTo = offsetTo
            };
        }

        if (syntaxNode is LocalFunctionStatementSyntax localFunctionStatementSyntax)
        {
            var declaredSymbol = projectWrapper.SemanticModel.GetDeclaredSymbol(localFunctionStatementSyntax, cancellationToken: ct);
            Debug.Assert(declaredSymbol is IMethodSymbol);

            tokens = SymbolInformationBuilder.BuildMethodSymbolInformation((IMethodSymbol)declaredSymbol);
        }
        else if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var declaredSymbol = projectWrapper.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax, cancellationToken: ct);
            Debug.Assert(declaredSymbol is IMethodSymbol);

            tokens = SymbolInformationBuilder.BuildMethodSymbolInformation((IMethodSymbol)declaredSymbol);
        }
        else if (syntaxNode is PredefinedTypeSyntax predefinedTypeSyntax)
        {
            var typeInfo = projectWrapper.SemanticModel.GetTypeInfo(predefinedTypeSyntax, cancellationToken: ct);
            Debug.Assert(typeInfo.Type is not null);

            tokens = SymbolInformationBuilder.BuildTypeSymbolInformation(typeInfo.Type);
        }
        else if (syntaxNode is ParameterSyntax parameterSyntax)
        {
            var declaredSymbol = projectWrapper.SemanticModel.GetDeclaredSymbol(parameterSyntax, cancellationToken: ct);
            Debug.Assert(declaredSymbol is IParameterSymbol);

            tokens = SymbolInformationBuilder.BuildParameterSymbolInformation((IParameterSymbol)declaredSymbol);
        }
        else if (syntaxNode is VariableDeclaratorSyntax variableDeclaratorSyntax)
        {
            // var x = 10; -> local variable int x
            if (variableDeclaratorSyntax.Initializer is not null)
            {
                var initializerValueTypeInfo = projectWrapper.SemanticModel.GetTypeInfo(variableDeclaratorSyntax.Initializer.Value, cancellationToken: ct);
                Debug.Assert(initializerValueTypeInfo.Type is not null);

                tokens =
                [
                    RoslynSyntaxToken.TextToken("local variable: "),
                    new RoslynSyntaxToken
                    {
                        Text = initializerValueTypeInfo.Type.GetDisplayText(),
                        TokenType = initializerValueTypeInfo.Type.GetTokenType()
                    },
                    RoslynSyntaxToken.Space,
                    RoslynSyntaxToken.TextToken(variableDeclaratorSyntax.Identifier.Text)
                ];
            }
            // int y; -> local variable int y
            else
            {
                var declaredSymbol = projectWrapper.SemanticModel.GetDeclaredSymbol(variableDeclaratorSyntax, cancellationToken: ct);
                // TODO: apparently hovering over field X int this code also gives VariableDeclaratorSyntax:
                // struct MyStruct
                // {
                //    public int X;
                // }
                Debug.Assert(declaredSymbol is ILocalSymbol);

                tokens = SymbolInformationBuilder.BuildLocalSymbolInformation((ILocalSymbol)declaredSymbol);
            }
        }
        else if (syntaxNode is IdentifierNameSyntax identifierNameSyntax)
        {
            var symbolInfo = projectWrapper.SemanticModel.GetSymbolInfo(identifierNameSyntax, cancellationToken: ct);

            if (symbolInfo.Symbol is not null)
            {
                tokens = SymbolInformationBuilder.BuildSymbolInformation(symbolInfo.Symbol);
            }
        }

        return new RoslynHoverResult
        {
            Tokens = tokens,
            OffsetFrom = offsetFrom,
            OffsetTo = offsetTo
        };
    }

    public static async Task<List<SemanticHighlightSpan>> GetSemanticHighlightsAsync(
        Guid projectId,
        string sourceCode,
        RoslynWorkspaceWrapper workspaceWrapper,
        CancellationToken ct = default)
    {
        var projectWrapper = await workspaceWrapper.GetOrCreateProjectAsync(projectId, ct);

        await projectWrapper.UpdateSourceCodeAndCompileAsync(sourceCode, ct);

        var results = await SemanticHighlighter.GetHighlightSpansAsync(projectWrapper.Document, ct);

        return results;
    }

    public static async Task<List<RoslynDiagnostic>> GetDiagnosticsAsync(
        Guid projectId,
        string sourceCode,
        RoslynWorkspaceWrapper workspaceWrapper,
        CancellationToken ct = default)
    {
        var projectWrapper = await workspaceWrapper.GetOrCreateProjectAsync(projectId, ct);

        await projectWrapper.UpdateSourceCodeAndCompileAsync(sourceCode, ct);

        var diagnostics = projectWrapper.EmitResult.Diagnostics;

        var results = diagnostics
            .Where(d => d.Severity != DiagnosticSeverity.Hidden)
            .Select(d => new RoslynDiagnostic
            {
                Id = d.Id,
                Message = d.GetMessage(),
                Severity = d.Severity,
                OffsetFrom = d.Location.SourceSpan.Start,
                OffsetTo = d.Location.SourceSpan.End
            })
            .ToList();

        return results;
    }
}
