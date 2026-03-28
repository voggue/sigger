namespace Sigger.Generator.Server;

/// <summary>
/// Controls exposure of the generated JSON schema (e.g. /sigger/sigger.json).
/// </summary>
public enum SiggerSchemaEndpointMode
{
    /// <summary>Schema is always served (legacy default).</summary>
    Always,

    /// <summary>Schema is served only when <see cref="Microsoft.Extensions.Hosting.IHostEnvironment"/> is Development.</summary>
    DevelopmentOnly,

    /// <summary>Schema endpoint is not registered.</summary>
    Disabled
}
