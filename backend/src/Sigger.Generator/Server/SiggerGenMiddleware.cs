using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Sigger.Generator.Server;

public class SiggerGenMiddleware
{
    private readonly string _json;

    internal SiggerGenMiddleware(string json)
    {
        _json = json;
    }

    public void HandleRequest(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(_json);
        });
    }
}
