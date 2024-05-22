using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Services;

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

var app = builder.Build();

#if DEBUG

app.UseSwagger();
app.UseSwaggerUI();

#endif // DEBUG

app.UseSerilogRequestLogging();

app.MapControllers();

await app.RunAsync();
