using DotnetAsm.Core.Models;

namespace DotnetAsm.Core.Interfaces;

public interface IAsmGenerator
{
    public Task<AsmGenerationResponse> GenerateAsm(AsmGenerationRequest request, CancellationToken ct);
}
