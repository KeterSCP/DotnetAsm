using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Models;

using Microsoft.AspNetCore.Mvc;

namespace DotnetAsm.Api.Controllers;

[ApiController]
public class AsmGeneratorController : ControllerBase
{
    private readonly IAsmGenerator _asmGenerator;

    public AsmGeneratorController(IAsmGenerator asmGenerator)
    {
        _asmGenerator = asmGenerator;
    }

    [HttpPost("/api/generate-asm")]
    public async Task<ActionResult<AsmGenerationResponse>> GenerateAsm([FromBody] AsmGenerationRequest request, CancellationToken ct)
    {
        return await _asmGenerator.GenerateAsm(request, ct);
    }
}