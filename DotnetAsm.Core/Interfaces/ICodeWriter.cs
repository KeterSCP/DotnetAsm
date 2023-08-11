namespace DotnetAsm.Core.Interfaces;

public interface ICodeWriter
{
    public Task WriteCodeAsync(string code);
}
