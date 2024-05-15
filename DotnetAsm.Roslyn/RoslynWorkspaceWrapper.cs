using System.Collections.Concurrent;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace DotnetAsm.Roslyn;

public class RoslynWorkspaceWrapper : IDisposable
{
    public AdhocWorkspace Workspace { get; }

    private static readonly string[] DefaultHostAssemblyNames =
    [
        "Microsoft.CodeAnalysis.Workspaces",
        "Microsoft.CodeAnalysis.CSharp.Workspaces",
        "Microsoft.CodeAnalysis.Features",
        "Microsoft.CodeAnalysis.CSharp.Features"
    ];

    private static readonly MefHostServices DefaultHost = MefHostServices.Create
    (
        assemblies: MefHostServices.DefaultAssemblies.Where(a => DefaultHostAssemblyNames.Contains(a.GetName().Name))
    );

    private static readonly IEnumerable<MetadataReference> DefaultMetadataReferences = GetDefaultMetadataReferences();

    // TODO: more advanced cache
    private readonly ConcurrentDictionary<Guid, RoslynProjectWrapper> _projects = new();

    public RoslynWorkspaceWrapper()
    {
        var workspace = new AdhocWorkspace(DefaultHost);
        Workspace = workspace;
    }

    public async Task<RoslynProjectWrapper> GetOrCreateProjectAsync(Guid id, CancellationToken ct)
    {
        if (_projects.TryGetValue(id, out var projectWrapper))
        {
            return projectWrapper;
        }

        var projectInfo = ProjectInfo.Create
        (
            id: ProjectId.CreateFromSerialized(id),
            version: VersionStamp.Default,
            name: $"TempProject-{id}",
            assemblyName: $"TempProject-{id}",
            language: LanguageNames.CSharp
        );

        projectInfo = projectInfo.WithMetadataReferences(DefaultMetadataReferences);

        var project = Workspace.AddProject(projectInfo);

        project = project.AddDocument("MainDocument.cs", SourceText.From(string.Empty, Encoding.UTF8)).Project;

        projectWrapper = await RoslynProjectWrapper.CreateAndCompileAsync(project, ct);
        _projects.TryAdd(id, projectWrapper);

        return projectWrapper;
    }

    public void Dispose()
    {
        Workspace.Dispose();
    }

    private static List<MetadataReference> GetDefaultMetadataReferences()
    {
        var rootPath = Path.Combine(
            Path.GetDirectoryName(new Uri(typeof(RoslynWorkspaceWrapper).Assembly.Location).LocalPath)!,
            "ref-assemblies"
        );

        string?[] assemblies = Directory.GetFiles(rootPath, "*.dll").Select(Path.GetFileNameWithoutExtension).ToArray();

        return assemblies.Select(assemblyName =>
        {
            var assemblyPath = Path.Combine(rootPath, $"{assemblyName}.dll");
            var documentationPath = Path.Combine(rootPath, $"{assemblyName}.xml");

            return (MetadataReference)MetadataReference.CreateFromFile(assemblyPath, MetadataReferenceProperties.Assembly, XmlDocumentationProvider.CreateFromFile(documentationPath));
        }).ToList();
    }
}
