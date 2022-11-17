using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Sigger.Generator.Server;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace Sigger.Generator;

public static class StartupExtensions
{
    public static IServiceCollection AddSigger(this IServiceCollection services,
        Action<SiggerGenOptions>? configure = null,
        Action<HubOptions>? configureHub = null)
    {
        var options = new SiggerGenOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        if (configureHub != null)
            services.AddSignalR(configureHub);
        else
            services.AddSignalR();
        return services;
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

        // map middleware
        var middleware = new SiggerGenMiddleware(options, doc);
        builder.Map(options.Path, middleware.HandleRequest);
        return builder;
    }
}