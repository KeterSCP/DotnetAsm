using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Services;

using Serilog;

namespace DotnetAsm.Api;

public class Startup
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _env = env;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSpaStaticFiles(options =>
        {
            options.RootPath = "dist";
        });

        services.AddOptions<CodeWriterSettings>()
            .Bind(_configuration.GetSection(CodeWriterSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ICodeWriter, CodeWriter>();
        services.AddScoped<IAsmGenerator, AsmGenerator>();
        services.AddSingleton(Log.Logger);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (_env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseAuthorization();

        app.UseSpaStaticFiles();

        app.UseSerilogRequestLogging();

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        app.UseSpa(spaBuilder =>
        {
            spaBuilder.Options.SourcePath = "../DotnetAsm.Frontend";
            if (_env.IsDevelopment())
            {
                spaBuilder.UseProxyToSpaDevelopmentServer(_configuration.GetValue<string>("FrontendUrl")!);
            }
        });
    }
}