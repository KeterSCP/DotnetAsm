using Microsoft.AspNetCore.Mvc;

using MonacoRoslynCompletionProvider;
using MonacoRoslynCompletionProvider.Api;

namespace DotnetAsm.Api.Controllers;

[ApiController]
public class RoslynCompletionController : ControllerBase
{
    [HttpPost("/api/completion/complete")]
    public async Task<TabCompletionResult[]> Complete([FromBody] TabCompletionRequest completionText)
    {
        var tabCompletionResults = await CompletitionRequestHandler.Handle(completionText);
        return tabCompletionResults;
    }

    [HttpPost("/api/completion/signature")]
    public async Task<SignatureHelpResult> Signature([FromBody] SignatureHelpRequest signatureHelpRequest)
    {
        var signatureHelpResult = await CompletitionRequestHandler.Handle(signatureHelpRequest);
        return signatureHelpResult;
    }

    [HttpPost("/api/completion/hover")]
    public async Task<HoverInfoResult> Hover([FromBody] HoverInfoRequest hoverInfoRequest)
    {
        var hoverInfoResult = await CompletitionRequestHandler.Handle(hoverInfoRequest);
        return hoverInfoResult;
    }

    [HttpPost("/api/completion/code-check")]
    public async Task<CodeCheckResult[]> CodeCheck([FromBody] CodeCheckRequest codeCheckRequest)
    {
        var codeCheckResults = await CompletitionRequestHandler.Handle(codeCheckRequest);
        return codeCheckResults;
    }
}
