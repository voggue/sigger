using System.Diagnostics.CodeAnalysis;
using System.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Sigger.Generator.Server;

namespace Sigger.UI;

public class SiggerUiMiddleware
{
    private readonly SiggerUiOptions _options;
    private readonly Dictionary<string, NameTypeContent> _fileContents = new(StringComparer.OrdinalIgnoreCase);
    private const string INDEX_RESOURCE_NAME = "Sigger.UI.Resources.index.html";
    private const string JS_RESOURCE_NAME = "Sigger.UI.Resources.build.bundle.js";
    private const string JSMAP_RESOURCE_NAME = "Sigger.UI.Resources.build.bundle.js.map";
    private const string CSS_RESOURCE_NAME = "Sigger.UI.Resources.build.bundle.css";
    private const string GLOBAL_CSS_RESOURCE_NAME = "Sigger.UI.Resources.global.css";
    private const string FAVICON_RESOURCE_NAME = "Sigger.UI.Resources.favicon.ico";
    private const string FAVICON_PNG_RESOURCE_NAME = "Sigger.UI.Resources.favicon.png";
    private const string LOGO_RESOURCE_NAME = "Sigger.UI.Resources.logo.png";


    public SiggerUiMiddleware(SiggerUiOptions options)
    {
        _options = options;
    }

    public void HandleRequest(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            if (TryGetFile(context, out var file))
            {
                context.Response.ContentType = file.ContentType;
                switch (file.Content)
                {
                    case byte[] data:
                        await context.Response.BodyWriter.WriteAsync(data);
                        break;
                    case string content:
                        await context.Response.WriteAsync(content);
                        break;
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        });
    }

    internal static Dictionary<string, NameType> FileMapping { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {"bundle.css", new(CSS_RESOURCE_NAME, "text/css", false)},
        {"global.css", new(GLOBAL_CSS_RESOURCE_NAME, "text/css", false)},
        {"bundle.js", new(JS_RESOURCE_NAME, "application/javascript", false)},
        {"bundle.js.map", new(JSMAP_RESOURCE_NAME, "application/json", false)},
        {"favicon.ico", new(FAVICON_RESOURCE_NAME, "image/icon", true)},
        {"favicon.png", new(FAVICON_PNG_RESOURCE_NAME, "image/png", true)},
        {"logo.png", new(LOGO_RESOURCE_NAME, "image/png", true)},
        {"index.html", new(INDEX_RESOURCE_NAME, "text/html", false)},
        {"index.htm", new(INDEX_RESOURCE_NAME, "text/html", false)},
        {"index", new(INDEX_RESOURCE_NAME, "text/html", false)},
        {"", new(INDEX_RESOURCE_NAME, "text/html", false)},
    };

    private bool TryGetFile(HttpContext httpContext, [MaybeNullWhen(false)] out NameTypeContent file)
    {
        var fullPath = httpContext.Request.Path.ToString();
        var fileName = Path.GetFileName(fullPath);

        if (!FileMapping.TryGetValue(fileName, out var fileContentType))
            fileContentType = new NameType(INDEX_RESOURCE_NAME, "text/html", false);

        // currently i use always the same index.html file.
        var resourceName = fileContentType.Name;
        var contentType = fileContentType.ContentType;

        if (_fileContents.TryGetValue(resourceName, out file))
        {
            if (!_options.IgnoreCaching)
                return true;
        }

        // load the file content from the resource.
        object? content;
        try
        {
            using var stream = GetType().Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new MissingManifestResourceException(resourceName + " not found");

            if (fileContentType.Binary)
            {
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                content = ms.ToArray();
            }
            else
            {
                using var rd = new StreamReader(stream);
                var stringContent = rd.ReadToEnd();
                content = resourceName.Equals(INDEX_RESOURCE_NAME, StringComparison.OrdinalIgnoreCase)
                    ? HandlePlaceHolders(httpContext, stringContent, fullPath)
                    : stringContent;
            }
        }
        catch (Exception e)
        {
            var eHtml = e.ToString().Replace("\n", "<br>");
            content = string.Format( eHtml);
            httpContext.Response.StatusCode = 404;
            contentType = "text/html";
        }

        _fileContents[resourceName] = file = new NameTypeContent(contentType, content);
        return true;
    }

    private string HandlePlaceHolders(HttpContext context, string content, string fullPath)
    {
        var genOptions = context.RequestServices.GetRequiredService<SiggerGenOptions>();

        var ext = Path.GetExtension(fullPath);

        // if no extension is available, we assume it is the index file
        // and the filename is the root-path of sigger
        var directory = string.IsNullOrWhiteSpace(ext)
            ? fullPath
            : $"{string.Join("/", fullPath.Split('/').SkipLast(1))}";

        directory = directory.TrimEnd('/');
        var uri = new Uri(context.Request.GetDisplayUrl());
        var url = uri.GetLeftPart(UriPartial.Authority) + genOptions.Path.ToLower();
        var sb = StringComparison.OrdinalIgnoreCase;
        return content
            .Replace("{{directory}}", directory, sb)
            .Replace("{{url}}", url, sb);
    }

    internal record NameType(string Name, string ContentType, bool Binary);

    private record NameTypeContent(string ContentType, object Content);
}