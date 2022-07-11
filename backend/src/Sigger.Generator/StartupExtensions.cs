using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sigger.Generator.Server;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace Sigger.Generator;

public static class StartupExtensions
{
    public static IServiceCollection AddSigger(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }

    public static WebApplicationBuilder AddSigger(this WebApplicationBuilder builder)
    {
        AddSigger(builder.Services);
        return builder;
    }

    public static IApplicationBuilder UseSigger(this WebApplication builder, Action<SiggerGenOptions>? configure = null)
    {
        var options = new SiggerGenOptions(builder);
        configure?.Invoke(options);

        // Search Hubs
        var generator = new SchemaGenerator(options.GenerationOptions);
        foreach (var hub in options.HubTypes)
            generator.AddHub(hub);

        // Autodiscover Hubs
        //  var asm = options.HubAssemblies.ToList().Union(new[] { Assembly.GetEntryAssembly() });
        // var th = typeof(Hub);
        // var thg = typeof(Hub<>);
        // foreach (var a in asm)
        // {
        //     if (a == null) continue;
        //     foreach (var t in a.GetTypes().Where(x => !x.IsAbstract && !x.IsInterface))
        //     {
        //         if (t.HasBaseClass(thg, th))
        //         {
        //             generator.AddHub(t);
        //         }
        //     }
        // }

        var doc = generator.CreateSchema();

        // map middleware
        var middleware = new SiggerGenMiddleware(options, doc);
        builder.Map(options.Path, middleware.HandleRequest);
        return builder;
    }
}