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
        File.Delete(_codeWriterSettings.WritePath);
        
        await using var fs = File.OpenWrite(_codeWriterSettings.WritePath);
        await using var fileWriter = new StreamWriter(fs)
        {
            AutoFlush = true
        };
        
        var codeStrMemory = code.AsMemory();

        var eolIdx = codeStrMemory.Span.IndexOf("\r\n"); 
        while (eolIdx != -1)
        {
            await fileWriter.WriteLineAsync(codeStrMemory[..eolIdx]);
            codeStrMemory = codeStrMemory[(eolIdx + 2)..]; 
            
            eolIdx = codeStrMemory.Span.IndexOf("\r\n");
        }
    
        await fileWriter.WriteLineAsync(codeStrMemory);
    }
}