using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Models;
using DotnetAsm.Core.Tools;

using Microsoft.Extensions.Options;

namespace DotnetAsm.Core.Services;

public class CliBasedAsmGenerator(ICodeWriter codeWriter, IOptions<CodeWriterSettings> codeWriterOptions) : IAsmGenerator
{
    private static readonly string _shell = OperatingSystem.IsLinux() ? "bash" : "cmd.exe";
    private static readonly Dictionary<int, AsmGenerationResponse> _cache = new();
    private readonly List<string> _asmJittedMethodsInfoList = new();
    private readonly StringBuilder _asmStringBuilder = new();
    private readonly CodeWriterSettings _codeWriterSettings = codeWriterOptions.Value;
    private readonly StringBuilder _stdErrStringBuilder = new();

    public async Task<AsmGenerationResponse> GenerateAsm(AsmGenerationRequest request, CancellationToken ct)
    {
        var hash = HashCode.Combine(request.CsharpCode, request.MethodName, request.UseReadyToRun, request.UsePgo, request.UseTieredCompilation, request.TargetFramework);
        if (_cache.TryGetValue(hash, out AsmGenerationResponse? cachedResponse))
        {
            return cachedResponse;
        }

        string tfm = request.TargetFramework switch
        {
            TargetFramework.Net70 => "net7.0",
            TargetFramework.Net80 => "net8.0",
            _ => throw new SwitchExpressionException(request.TargetFramework)
        };

        await codeWriter.WriteCodeAsync(request.CsharpCode);

         using var dotnetBuildProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {Path.GetDirectoryName(_codeWriterSettings.WritePath)} -c Release --no-self-contained --nologo --no-dependencies -f {tfm}",
                EnvironmentVariables =
                {
                    ["SuppressNETCoreSdkPreviewMessage"] = "true",
                    ["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1",
                    ["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"
                },
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };

        dotnetBuildProcess.Start();

        // See https://github.com/dotnet/runtime/issues/46382
        var buildStdOutTask = dotnetBuildProcess.StandardOutput.ReadToEndAsync(ct);

        await Measure.AsyncAction(() => dotnetBuildProcess.WaitForExitAsync(ct), "dotnetBuildProcess.WaitForExitAsync");

        var buildStdOut = await buildStdOutTask;

        if (dotnetBuildProcess.ExitCode != 0)
        {
            return new AsmGenerationResponse
            {
                Asm = "",
                Errors = buildStdOut
            };
        }

        string builtDllPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(_codeWriterSettings.WritePath)!, "bin", "Release", tfm, "DotnetAsm.Sandbox.dll");
        string shellArgs = OperatingSystem.IsLinux() ? $"dotnet {builtDllPath}" : $"/c dotnet {builtDllPath}";

        using var shellProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _shell,
                EnvironmentVariables =
                {
                    ["DOTNET_JitDisasm"] = $"{request.MethodName}",
                    ["DOTNET_JitDisasmSummary"] = "1",
                    ["DOTNET_ReadyToRun"] = request.UseReadyToRun ? "1" : "0",
                    ["DOTNET_TieredPGO"] = request.UsePgo ? "1" : "0",
                    ["DOTNET_TieredCompilation"] = request.UsePgo || request.UseTieredCompilation ? "1" : "0"
                },
                Arguments = shellArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        shellProcess.OutputDataReceived += HandleProcessStdOut;
        shellProcess.ErrorDataReceived += HandleProcessStdErr;

        try
        {
            shellProcess.Start();
            shellProcess.BeginOutputReadLine();
            shellProcess.BeginErrorReadLine();

            await Measure.AsyncAction(() => shellProcess.WaitForExitAsync(ct), "shellProcess.WaitForExitAsync");
        }
        finally
        {
            shellProcess.OutputDataReceived -= HandleProcessStdOut;
            shellProcess.ErrorDataReceived -= HandleProcessStdErr;
        }

        var response = new AsmGenerationResponse
        {
            Asm = _asmStringBuilder.ToString(),
            AsmSummary = _asmJittedMethodsInfoList,
            Errors = _stdErrStringBuilder.Length > 0 ? _stdErrStringBuilder.ToString() : null
        };

        _cache.Add(hash, response);
        return response;
    }

    private void HandleProcessStdErr(object sender, DataReceivedEventArgs args)
    {
        if (string.IsNullOrEmpty(args.Data))
        {
            return;
        }

        _stdErrStringBuilder.AppendLine(args.Data);
    }

    private void HandleProcessStdOut(object sender, DataReceivedEventArgs args)
    {
        if (string.IsNullOrEmpty(args.Data))
        {
            return;
        }

        if (args.Data.Contains("JIT compiled"))
        {
            _asmJittedMethodsInfoList.Add(args.Data);
        }
        else
        {
            _asmStringBuilder.AppendLine(args.Data);
            if (IsAsmSectionEnd(args.Data))
            {
                _asmStringBuilder.AppendLine();
            }
        }
    }

    private static bool IsAsmSectionEnd(ReadOnlySpan<char> line)
    {
        return line.StartsWith("; Total bytes of code");
    }
}
