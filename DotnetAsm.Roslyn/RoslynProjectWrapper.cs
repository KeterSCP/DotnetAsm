using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace DotnetAsm.Roslyn;

/// <summary>
/// Project with a single document
/// </summary>
public class RoslynProjectWrapper
{
    public Project Project { get; private set; }
    public Document Document => Project.Documents.Single();
    public SemanticModel? SemanticModel { get; private set; }
    public EmitResult EmitResult { get; private set; }

    private RoslynProjectWrapper(Project project, SemanticModel? semanticModel, EmitResult emitResult)
    {
        Project = project;
        SemanticModel = semanticModel;
        EmitResult = emitResult;
    }

    public static async Task<RoslynProjectWrapper> CreateAndCompileAsync(Project project, CancellationToken ct)
    {
        var compilation = await project.GetCompilationAsync(ct);

        if (compilation is null)
        {
            throw new InvalidOperationException("Failed to get compilation");
        }

        using var memoryStream = new MemoryStream();

        var emitResult = compilation.Emit(memoryStream, cancellationToken: ct);

        var syntaxTree = compilation.SyntaxTrees.SingleOrDefault();

        var semanticModel = syntaxTree is null ? null : compilation.GetSemanticModel(syntaxTree);

        return new RoslynProjectWrapper(project, semanticModel, emitResult);
    }

    [MemberNotNull(nameof(SemanticModel))]
    public async Task UpdateSourceCodeAndCompileAsync(string source, CancellationToken ct)
    {
        var document = Project.Documents.Single();
        var newDocument = document.WithText(SourceText.From(source, Encoding.UTF8));

        using var memoryStream = new MemoryStream();
        var compilation = await newDocument.Project.GetCompilationAsync(ct);

        if (compilation is null)
        {
            throw new InvalidOperationException("Failed to get compilation");
        }

        var emitResult = compilation.Emit(memoryStream, cancellationToken: ct);
        var semanticModel = compilation.GetSemanticModel(compilation.SyntaxTrees.First());

        Project = newDocument.Project;
        SemanticModel = semanticModel;
        EmitResult = emitResult;
    }
}
