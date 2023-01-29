using System.Diagnostics;
using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;

using Microsoft.Extensions.Options;

namespace DotnetAsm.Core.Services;

public class CodeWriter : ICodeWriter
{
    private readonly CodeWriterSettings _codeWriterSettings;

    public CodeWriter(IOptions<CodeWriterSettings> codeWriterOptions)
    {
        _codeWriterSettings = codeWriterOptions.Value;
    }

    public async Task WriteCodeAsync(string code)
    {
        await File.WriteAllTextAsync(_codeWriterSettings.WritePath, code);
    }
}