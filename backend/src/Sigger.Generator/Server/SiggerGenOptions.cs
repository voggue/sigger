using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Generator.Server;

public class SiggerGenOptions
{
    internal SiggerGenOptions()
    {
    }

    private string? _path;

    // private readonly HashSet<Assembly> _hubAssemblies = new();
    private readonly Dictionary<Type, string> _hubTypes = new();

    private List<Action<IEndpointRouteBuilder>> _hubRegistrationActions = new();

    public SchemaGeneratorOptions GenerationOptions { get; } = new();

    /// <summary>
    /// When to expose the schema JSON endpoint. Prefer <see cref="SiggerSchemaEndpointMode.DevelopmentOnly"/> in production.
    /// </summary>
    public SiggerSchemaEndpointMode SchemaEndpointMode { get; set; } = SiggerSchemaEndpointMode.Always;

    /// <summary>
    /// The path to call the sigger definition
    /// </summary>
    public string Path
    {
        get => _path ?? "/sigger/sigger.json";
        set => _path = value;
    }

    // /// <summary>
    // /// List of Assemblies where defined Hubs should be searched
    // /// </summary>
    // public IReadOnlySet<Assembly> HubAssemblies => _hubAssemblies;

    /// <summary>
    /// List of Hub-Types
    /// </summary>
    public IReadOnlyDictionary<Type, string> HubTypes => _hubTypes;

    /// <summary>
    /// The path to call the sigger definition
    /// </summary>
    public SiggerGenOptions WithPath(string path)
    {
        _path = path;
        return this;
    }

    /// <summary>
    /// Configure the Generation Options
    /// </summary>
    public SiggerGenOptions WithGeneratorOptions(Action<SchemaGeneratorOptions> configure)
    {
        configure.Invoke(GenerationOptions);
        return this;
    }

    /// <summary>
    /// Configure when the schema endpoint is exposed.
    /// </summary>
    public SiggerGenOptions WithSchemaEndpointMode(SiggerSchemaEndpointMode mode)
    {
        SchemaEndpointMode = mode;
        return this;
    }

    /// <summary>
    /// Register a Hub
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public SiggerGenOptions WithHub<T>(string endpoint) where T : Hub
    {
        _hubTypes.Add(typeof(T), endpoint);
        _hubRegistrationActions.Add(p => p.MapHub<T>(endpoint));
        return this;
    }

    internal void MapHubs(IEndpointRouteBuilder builder)
    {
        foreach (var registration in _hubRegistrationActions)
        {
            registration.Invoke(builder);
        }
    }
}