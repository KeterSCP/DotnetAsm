using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;

using Microsoft.Extensions.Options;

namespace DotnetAsm.Core.Services;

public class CodeWriter(IOptions<CodeWriterSettings> codeWriterOptions) : ICodeWriter
{
    private readonly CodeWriterSettings _codeWriterSettings = codeWriterOptions.Value;

    public async Task WriteCodeAsync(string code)
    {
        await File.WriteAllTextAsync(_codeWriterSettings.WritePath, code);
    }
}
