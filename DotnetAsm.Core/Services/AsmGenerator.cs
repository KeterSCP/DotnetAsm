﻿using System.Diagnostics;
using System.Text;

using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Models;

using Microsoft.Extensions.Options;

namespace DotnetAsm.Core.Services;

public class AsmGenerator : IAsmGenerator
{
    private readonly ICodeWriter _codeWriter;
    private readonly CodeWriterSettings _codeWriterSettings;
    private readonly StringBuilder _asmStringBuilder;
    private readonly StringBuilder _stdErrStringBuilder;
    private StringBuilder? _asmSummaryStringBuilder;
    private bool _asmSection;

    public AsmGenerator(ICodeWriter codeWriter, IOptions<CodeWriterSettings> codeWriterOptions)
    {
        _codeWriter = codeWriter;
        _codeWriterSettings = codeWriterOptions.Value;
        _asmStringBuilder = new StringBuilder();
        _stdErrStringBuilder = new StringBuilder();
    }

    public async Task<AsmGenerationResponse> GenerateAsm(AsmGenerationRequest request, CancellationToken ct)
    {
        if (request.GenerateSummary)
        {
            _asmSummaryStringBuilder = new StringBuilder();
        }
        
        await _codeWriter.WriteCodeAsync(request.CsharpCode);

        using var dotnetBuildProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {Path.GetDirectoryName(_codeWriterSettings.WritePath)} -c Release --no-self-contained /p:WarningLevel=0 /p:TreatWarningsAsErrors=false",
                EnvironmentVariables =
                {
                    ["DOTNET_TC_QuickJitForLoops"] = "1",
                    ["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1",
                    ["DOTNET_MULTILEVEL_LOOKUP"] = "1",
                    ["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"
                },
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };
        
        dotnetBuildProcess.Start();
        
        await dotnetBuildProcess.WaitForExitAsync(ct);
            var errors = await dotnetBuildProcess.StandardOutput.ReadToEndAsync(ct);
        if (dotnetBuildProcess.ExitCode != 0)
        {
            return new AsmGenerationResponse
            {
                Asm = "",
                Errors = errors
            };
        }

        var dllPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(_codeWriterSettings.WritePath)!, "bin", "Release", "net7.0", "DotnetAsm.Sandbox.dll");
        
        var shell = OperatingSystem.IsLinux() ? "bash" : "cmd.exe";
        var shellArgs = OperatingSystem.IsLinux() ? $"dotnet {dllPath}" : $"/c dotnet {dllPath}";

        using var shellProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = shell,
                EnvironmentVariables =
                {
                    ["DOTNET_JitDisasm"] = request.MethodName,
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
        
            await shellProcess.WaitForExitAsync(ct);
        }
        finally
        {
            shellProcess.OutputDataReceived -= HandleProcessStdOut;
            shellProcess.ErrorDataReceived -= HandleProcessStdErr;
        }

        return new AsmGenerationResponse
        {
            Asm = _asmStringBuilder.ToString(),
            Errors = _stdErrStringBuilder.Length > 0 ? _stdErrStringBuilder.ToString() : null
        };
    }

    private void HandleProcessStdErr(object sender, DataReceivedEventArgs args)
    {
        if (string.IsNullOrEmpty(args.Data)) return;
        _stdErrStringBuilder.AppendLine(args.Data);
    }

    private void HandleProcessStdOut(object sender, DataReceivedEventArgs args)
    {
        if (string.IsNullOrEmpty(args.Data)) return;

        if (!_asmSection)
        {
            _asmSection = IsAsmSectionStart(args.Data);

            if (_asmSection)
            {
                _asmStringBuilder.AppendLine(args.Data);
            }
        }
        else
        {
            _asmStringBuilder.AppendLine(args.Data);
            _asmSection = !IsAsmSectionEnd(args.Data);
            if (!_asmSection)
            {
                _asmStringBuilder.AppendLine();
            }
        }
    }

    private bool IsAsmSectionStart(string line)
    {
        return line.AsSpan().StartsWith("; Assembly listing for method");
    }
    
    private bool IsAsmSectionEnd(string line)
    {
        return line.AsSpan().StartsWith("; Total bytes of code");
    }
}