using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Models;

using Microsoft.AspNetCore.Mvc;

namespace DotnetAsm.Api.Controllers;

[ApiController]
public class AsmGeneratorController(IAsmGenerator asmGenerator) : ControllerBase
{
    [HttpPost("/api/generate-asm")]
    public async Task<ActionResult<AsmGenerationResponse>> GenerateAsm([FromBody] AsmGenerationRequest request, CancellationToken ct)
    {
        return await asmGenerator.GenerateAsm(request, ct);
    }
}
