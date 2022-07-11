using System.Diagnostics.CodeAnalysis;
using System.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Sigger.UI;

public class SiggerUiMiddleware
{
    private readonly SiggerUiOptions _options;
    private readonly Dictionary<string, NameTypeContent> _fileContents = new(StringComparer.OrdinalIgnoreCase);
    private const string INDEX_RESSOURCE_NAME = "Sigger.UI.Resources.index.html";
    private const string THEME_RESSOURCE_NAME = "Sigger.UI.Resources.theme.css";
    private const string FAVICON_RESSOURCE_NAME = "Sigger.UI.Resources.favicon.ico";
    private const string SCREEN_CSS_RESSOURCE_NAME = "Sigger.UI.Resources.screen.css";
    private const string STYLE_CSS_RESSOURCE_NAME = "Sigger.UI.Resources.style.css";
    private const string SCRIPT_RESSOURCE_NAME = "Sigger.UI.Resources.script.js";
    private const string IMAGE_LOGO_RESSOURCE_NAME = "Sigger.UI.Resources.logo_small.png";

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
        { "theme.css", new(THEME_RESSOURCE_NAME, "text/css", false) },
        { "style.css", new(STYLE_CSS_RESSOURCE_NAME, "text/css", false) },
        { "screen.css", new(SCREEN_CSS_RESSOURCE_NAME, "text/css", false) },
        { "script.js", new(SCRIPT_RESSOURCE_NAME, "application/javascript", false) },
        { "favicon.ico", new(FAVICON_RESSOURCE_NAME, "image/icon", true) },
        { "logo.png", new(IMAGE_LOGO_RESSOURCE_NAME, "image/png", true) },
        { "index.html", new(INDEX_RESSOURCE_NAME, "text/html", false) },
        { "index.htm", new(INDEX_RESSOURCE_NAME, "text/html", false) },
        { "index", new(INDEX_RESSOURCE_NAME, "text/html", false) },
    };

    private bool TryGetFile(HttpContext httpContext, [MaybeNullWhen(false)] out NameTypeContent file)
    {
        var fullPath = httpContext.Request.Path.ToString();
        var fileName = Path.GetFileName(fullPath);

        if (!_fileMapping.TryGetValue(fileName, out var fileContentType))
            fileContentType = new NameType(INDEX_RESSOURCE_NAME, "text/html", false);

        // currently i use always the same index.html file.
        var resourceName = fileContentType.Name;
        var contentType = fileContentType.ContentType;

        if (_fileContents.TryGetValue(resourceName, out file))
            return true;

        // load the file content from the resource.
        object? content = null;
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
                content = resourceName.Equals(INDEX_RESSOURCE_NAME, StringComparison.OrdinalIgnoreCase)
                    ? HandlePlaceHolders(stringContent, fullPath)
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

        _fileContents[resourceName] = file = new NameTypeContent(resourceName, contentType, content ?? "#EMPTY#");
        return true;
    }

    private string HandlePlaceHolders(string content, string fullPath)
    {
        var ext = Path.GetExtension(fullPath);

        // if no extension is available, we assume it is the index file
        // and the filename is the root-path of sigger
        var directory = string.IsNullOrWhiteSpace(ext)
            ? fullPath
            : $"{string.Join("/", fullPath.Split('/').SkipLast(1))}";

        directory = directory.TrimEnd('/');

        return content.Replace("{{directory}}", directory, StringComparison.OrdinalIgnoreCase);
    }

    private record NameType(string Name, string ContentType, bool Binary);

    private record NameTypeContent(string Name, string ContentType, object Content);
}