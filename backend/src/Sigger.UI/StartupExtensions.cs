// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Sigger.UI;

public static class StartupExtensions
{
    public static IApplicationBuilder UseSiggerUi(this WebApplication builder, Action<SiggerUiOptions>? configure = null)
    {
        var options = new SiggerUiOptions(builder);
        configure?.Invoke(options);

        // map middleware
        var middleware = new SiggerUiMiddleware(options);
        builder.MapWhen(ctx => Precondition(ctx, options), middleware.HandleRequest);
        return builder;
    }

    private static readonly HashSet<string> _supportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".css",
        ".html",
        ".htm",
        ".png",
        ".ico",
        ".jpg",
        ".js"
    };

    private static bool Precondition(HttpContext ctx, SiggerUiOptions options)
    {
        var fullPath = ctx.Request.Path.ToString();
        if (!fullPath.StartsWith(options.Path, StringComparison.OrdinalIgnoreCase))
            return false;

        var ext = Path.GetExtension(fullPath);
        return string.IsNullOrEmpty(ext) || _supportedExtensions.Contains(ext);
    }
}