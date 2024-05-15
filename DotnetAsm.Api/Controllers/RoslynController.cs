using DotnetAsm.Api.Models;
using DotnetAsm.Roslyn;
using DotnetAsm.Roslyn.Completion.Models;
using DotnetAsm.Roslyn.Hover.Models;
using DotnetAsm.Roslyn.SemanticHighlight.Models;

using Microsoft.AspNetCore.Mvc;


namespace DotnetAsm.Api.Controllers;

[ApiController]
public class RoslynController : ControllerBase
{
    private readonly RoslynWorkspaceWrapper _workspace;

    public RoslynController(RoslynWorkspaceWrapper workspace)
    {
        _workspace = workspace;
    }

    [HttpPost("/api/roslyn/complete")]
    public async Task<IEnumerable<RoslynCompletionResult>> Complete([FromBody] RoslynCompletionRequest completionRequest)
    {
        var completions = await RoslynProvider.GetCompletionsAsync
        (
            completionRequest.ProjectId,
            completionRequest.Code,
            completionRequest.Position,
            _workspace
        );

        return completions;
    }

    // [HttpPost("/api/roslyn/signature")]
    // public async Task<SignatureHelpResult> Signature([FromBody] SignatureHelpRequest signatureHelpRequest)
    // {
    //     var signatureHelpResult = await RoslynMonacoRequestHandler.Handle(signatureHelpRequest);
    //     return signatureHelpResult;
    // }
    //
    [HttpPost("/api/roslyn/hover")]
    public async Task<RoslynHoverResult> Hover([FromBody] RoslynHoverRequest hoverInfoRequest)
    {
        var hoverInfoResult = await RoslynProvider.GetHoverAsync(
            hoverInfoRequest.ProjectId,
            hoverInfoRequest.Code,
            hoverInfoRequest.Position,
            _workspace
        );
        return hoverInfoResult;
    }
    //
    // [HttpPost("/api/roslyn/code-check")]
    // public async Task<CodeCheckResult[]> CodeCheck([FromBody] CodeCheckRequest codeCheckRequest)
    // {
    //     var codeCheckResults = await RoslynMonacoRequestHandler.Handle(codeCheckRequest);
    //     return codeCheckResults;
    // }

    [HttpPost("/api/roslyn/semantic-highlight")]
    public async Task<IEnumerable<SemanticHighlightSpan>> SemanticHighlight([FromBody] RoslynSemanticHighlightRequest request)
    {
        var highlights = await RoslynProvider.GetSemanticHighlightsAsync(
            request.ProjectId,
            request.Code,
            _workspace
        );
        return highlights;
    }
}
