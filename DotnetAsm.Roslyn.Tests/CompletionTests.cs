using DotnetAsm.Roslyn.Completion.Models;

using FluentAssertions;

using Microsoft.CodeAnalysis;

namespace DotnetAsm.Roslyn.Tests;

public class CompletionTests
{
    [Fact(DisplayName = "Should correctly generate completions for Guid")]
    public async Task ShouldCorrectlyGenerateCompletionsForGuid()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        using var workspace = new RoslynWorkspaceWrapper();

        const string sourceCode =
            """
            using System;
            Guid.
            """;

        var position = sourceCode.IndexOf("Guid.", StringComparison.Ordinal) + 5;

        // Act
        var completions = await RoslynProvider.GetCompletionsAsync(projectId, sourceCode, position, workspace);

        // Assert
        completions.Should().BeEquivalentTo(
        [
            new RoslynCompletionResult
            {
                DisplayText = "Empty",
                Description = """
                              (field) static readonly Guid Guid.Empty
                              A read-only instance of the Guid structure whose value is all zeros.
                              """,
                SymbolKind = SymbolKind.Field,
            },
            new RoslynCompletionResult
            {
                DisplayText = "Equals",
                Description = """
                              bool object.Equals(object? objA, object? objB)
                              Determines whether the specified object instances are considered equal.
                              """,
                SymbolKind = SymbolKind.Method
            },
            new RoslynCompletionResult
            {
                DisplayText = "NewGuid",
                Description = """
                              Guid Guid.NewGuid()
                              Initializes a new instance of the Guid structure.
                              """,
                SymbolKind = SymbolKind.Method
            },
            new RoslynCompletionResult
            {
                DisplayText = "Parse",
                Description = """
                              Guid Guid.Parse(ReadOnlySpan<char> input) (+ 3 overloads)
                              Converts a read-only character span that represents a GUID to the equivalent Guid structure.
                              """,
                SymbolKind = SymbolKind.Method
            },
            new RoslynCompletionResult
            {
                DisplayText = "ParseExact",
                Description = """
                              Guid Guid.ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format) (+ 1 overload)
                              Converts the character span representation of a GUID to the equivalent Guid structure, provided that the string is in the specified format.
                              """,
                SymbolKind = SymbolKind.Method
            },
            new RoslynCompletionResult
            {
                DisplayText = "ReferenceEquals",
                Description = """
                              bool object.ReferenceEquals(object? objA, object? objB)
                              Determines whether the specified object instances are the same instance.
                              """,
                SymbolKind = SymbolKind.Method
            },
            new RoslynCompletionResult
            {
                DisplayText = "TryParse",
                Description = """
                              bool Guid.TryParse(ReadOnlySpan<char> input, out Guid result) (+ 3 overloads)
                              Converts the specified read-only span of characters containing the representation of a GUID to the equivalent Guid structure.
                              """,
                SymbolKind = SymbolKind.Method
            },
            new RoslynCompletionResult
            {
                DisplayText = "TryParseExact",
                Description = """
                              bool Guid.TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out Guid result) (+ 1 overload)
                              Converts span of characters representing the GUID to the equivalent Guid structure, provided that the string is in the specified format.
                              """,
                SymbolKind = SymbolKind.Method
            }
        ]);
    }
}
