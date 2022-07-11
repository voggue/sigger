using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Generator.Server;

public class SiggerGenOptions
{
    private readonly WebApplication _builder;

    internal SiggerGenOptions(WebApplication builder)
    {
        _builder = builder;
    }

    private string? _path;

    // private readonly HashSet<Assembly> _hubAssemblies = new();
    private readonly HashSet<Type> _hubTypes = new();

    public SchemaGeneratorOptions GenerationOptions { get; } = new();

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
    public IReadOnlySet<Type> HubTypes => _hubTypes;

    /// <summary>
    /// The path to call the sigger definition
    /// </summary>
    public SiggerGenOptions WithPath(string path)
    {
        _path = path;
        return this;
    }

    // /// <summary>
    // /// The Assemblies where defined Hubs should be searched
    // /// </summary>
    // public SiggerGenOptions WithAssembly(params Assembly[] assemblies)
    // {
    //     foreach (var a in assemblies)
    //         _hubAssemblies.Add(a);
    //     return this;
    // }

    /// <summary>
    /// The Assemblies where defined Hubs should be searched
    /// </summary>
    public SiggerGenOptions WithHubTypes(params Type[] hubTypes)
    {
        foreach (var a in hubTypes)
            _hubTypes.Add(a);
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
    /// Register a Hub
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public SiggerGenOptions WithHub<T>(string endpoint) where T : Hub
    {
        if (_hubTypes.Add(typeof(T)))
            _builder.MapHub<T>(endpoint);
        return this;
    }
}