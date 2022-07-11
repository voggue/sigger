using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sigger.Schema;

namespace Sigger.Generator.Server;

public class SiggerGenMiddleware
{
    private readonly SiggerGenOptions _options;
    private readonly SchemaDocument _schema;

    internal SiggerGenMiddleware(SiggerGenOptions options, SchemaDocument schema)
    {
        _options = options;
        _schema = schema;
    }

    public void HandleRequest(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var json = _schema.ToJson();
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        });
    }
}