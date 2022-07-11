using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Text.RegularExpressions;
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
    private const string THEME_RESOURCE_NAME = "Sigger.UI.Resources.theme.css";
    private const string FAVICON_RESOURCE_NAME = "Sigger.UI.Resources.favicon.ico";
    private const string SCREEN_CSS_RESOURCE_NAME = "Sigger.UI.Resources.screen.css";
    private const string STYLE_CSS_RESOURCE_NAME = "Sigger.UI.Resources.style.css";
    private const string SCRIPT_RESOURCE_NAME = "Sigger.UI.Resources.sigger.js";
    private const string IMAGE_LOGO_RESOURCE_NAME = "Sigger.UI.Resources.logo_small.png";

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

    private readonly Dictionary<string, NameType> _fileMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        {"theme.css", new(THEME_RESOURCE_NAME, "text/css", false)},
        {"style.css", new(STYLE_CSS_RESOURCE_NAME, "text/css", false)},
        {"screen.css", new(SCREEN_CSS_RESOURCE_NAME, "text/css", false)},
        {"sigger.js", new(SCRIPT_RESOURCE_NAME, "application/javascript", false)},
        {"favicon.ico", new(FAVICON_RESOURCE_NAME, "image/icon", true)},
        {"logo.png", new(IMAGE_LOGO_RESOURCE_NAME, "image/png", true)},
        {"index.html", new(INDEX_RESOURCE_NAME, "text/html", false)},
        {"index.htm", new(INDEX_RESOURCE_NAME, "text/html", false)},
        {"index", new(INDEX_RESOURCE_NAME, "text/html", false)},
    };

    private bool TryGetFile(HttpContext httpContext, [MaybeNullWhen(false)] out NameTypeContent file)
    {
        var fullPath = httpContext.Request.Path.ToString();
        var fileName = Path.GetFileName(fullPath);

        if (!_fileMapping.TryGetValue(fileName, out var fileContentType))
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
            var resources = GetType().Assembly.GetManifestResourceNames();
            const string err = "<html>" +
                               "<head><title>Sigger UI Error</title></head>" +
                               "<body>" +
                               "<h1>Error</h1>" +
                               "<p style='color: red; font-size: 1.2em;'>{0}</p>" +
                               "<h2>Available</h2>" +
                               "<p>{1}</p>" +
                               "</body>" +
                               "</html>";

            var eHtml = e.ToString().Replace("\n", "<br>");
            var resourcesHtml = string.Join("<br>", resources);
            content = string.Format(err, eHtml, resourcesHtml);
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

    private record NameType(string Name, string ContentType, bool Binary);

    private record NameTypeContent(string ContentType, object Content);
}