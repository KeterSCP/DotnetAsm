using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Services;
using DotnetAsm.Roslyn;

using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog(loggerConfiguration => loggerConfiguration
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddCors();

#if DEBUG

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endif // DEBUG

builder.Services.AddOptions<CodeWriterSettings>()
    .Bind(builder.Configuration.GetSection(CodeWriterSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<ICodeWriter, CodeWriter>();
builder.Services.AddSingleton<IAsmGenerator, CliBasedAsmGenerator>();
builder.Services.AddSingleton<RoslynWorkspaceWrapper>();

var app = builder.Build();

#if DEBUG

app.UseSwagger();
app.UseSwaggerUI();

#endif // DEBUG

app.UseSerilogRequestLogging();

app.MapControllers();

app.UseCors(cors =>
{
    cors.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(Environment.GetEnvironmentVariable("FRONTEND_URL") ?? throw new InvalidOperationException("FRONTEND_URL environment variable is not set."));
});

await app.RunAsync();
