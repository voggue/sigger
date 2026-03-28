// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sigger.UI;

public static class StartupExtensions
{
    public static IApplicationBuilder UseSiggerUi(this WebApplication builder, Action<SiggerUiOptions>? configure = null)
    {
        var options = new SiggerUiOptions(builder);
        configure?.Invoke(options);

        var env = builder.Services.GetRequiredService<IWebHostEnvironment>();
        if (options.Visibility == SiggerUiVisibility.DevelopmentOnly && !env.IsDevelopment())
            return builder;

        var middleware = new SiggerUiMiddleware(options);
        builder.MapWhen(ctx => Precondition(ctx, options), middleware.HandleRequest);
        return builder;
    }
    
    private static bool Precondition(HttpContext ctx, SiggerUiOptions options)
    {
        var fullPath = ctx.Request.Path.ToString();
        if (!fullPath.StartsWith(options.Path, StringComparison.OrdinalIgnoreCase))
            return false;

        var relativePath = fullPath.Substring(options.Path.Length).TrimStart('/');
        return SiggerUiMiddleware.FileMapping.ContainsKey(relativePath);
    }
}