using DotnetAsm.Core.ConfigOptions;
using DotnetAsm.Core.Interfaces;
using DotnetAsm.Core.Services;

using Serilog;

namespace DotnetAsm.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSpaStaticFiles(options => { options.RootPath = "dist"; });

        services.AddOptions<CodeWriterSettings>()
            .Bind(configuration.GetSection(CodeWriterSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ICodeWriter, CodeWriter>();
        services.AddScoped<IAsmGenerator, CliBasedAsmGenerator>();
        services.AddSingleton(Log.Logger);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseSpaStaticFiles();

        app.UseSerilogRequestLogging();

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        app.UseSpa(spaBuilder =>
        {
            spaBuilder.Options.SourcePath = "../DotnetAsm.Frontend";
            if (env.IsDevelopment())
            {
                spaBuilder.UseProxyToSpaDevelopmentServer(configuration.GetValue<string>("FrontendUrl")!);
            }
        });
    }
}
