using FluentAssertions;

namespace DotnetAsm.Roslyn.Tests;

public class RoslynWorkspaceWrapperTests
{
    [Fact(DisplayName = "CreateProject should create project")]
    public async Task CreateProject_ShouldCreateProject()
    {
        using var workspace = new RoslynWorkspaceWrapper();
        var projectWrapper = await workspace.GetOrCreateProjectAsync(Guid.NewGuid(), CancellationToken.None);

        projectWrapper.Project.Should().NotBeNull();
    }
}
