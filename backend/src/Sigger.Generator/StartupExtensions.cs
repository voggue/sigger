using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sigger.Generator.Server;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace Sigger.Generator;

public static class StartupExtensions
{
    public static ISignalRServerBuilder AddSigger(this IServiceCollection services,
        Action<SiggerGenOptions>? configure = null,
        Action<HubOptions>? configureHub = null)
    {
        var options = new SiggerGenOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        return configureHub != null ? services.AddSignalR(configureHub) : services.AddSignalR();
    }

    public static WebApplicationBuilder AddSigger(this WebApplicationBuilder builder,
        Action<SiggerGenOptions>? configure = null,
        Action<HubOptions>? configureHub = null)
    {
        AddSigger(builder.Services, configure, configureHub);
        return builder;
    }

    public static IApplicationBuilder UseSigger(this WebApplication builder)
    {
        var options = builder.Services.GetRequiredService<SiggerGenOptions>();
        var generator = new SchemaGenerator(options.GenerationOptions);

        options.MapHubs(builder);
        foreach (var hub in options.HubTypes)
            generator.AddHub(hub.Key, hub.Value);

        var doc = generator.CreateSchema();
        var json = doc.ToJson();

        if (options.SchemaEndpointMode != SiggerSchemaEndpointMode.Disabled)
        {
            var env = builder.Services.GetRequiredService<IWebHostEnvironment>();
            var middleware = new SiggerGenMiddleware(json);
            builder.Map(options.Path, app =>
            {
                if (options.SchemaEndpointMode == SiggerSchemaEndpointMode.DevelopmentOnly
                    && !env.IsDevelopment())
                {
                    app.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("Sigger schema is not exposed in this environment.");
                    });
                    return;
                }

                middleware.HandleRequest(app);
            });
        }

        return builder;
    }
}