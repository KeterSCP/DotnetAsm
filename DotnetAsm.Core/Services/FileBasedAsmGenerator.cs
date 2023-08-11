using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Models;

namespace DotnetAsm.Core.Services;

// TODO: As an alternative to the CLI, use DOTNET_JitStdOutFile to write JIT output to specified file
public class FileBasedAsmGenerator : IAsmGenerator
{
    public Task<AsmGenerationResponse> GenerateAsm(AsmGenerationRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
