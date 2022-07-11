using Microsoft.AspNetCore.Builder;

namespace Sigger.UI;

public class SiggerUiOptions
{
    private readonly WebApplication _builder;
    private string? _path;
    public bool IgnoreCaching { get; private set; }

    internal SiggerUiOptions(WebApplication builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// The path to call the sigger ui
    /// </summary>
    public string Path
    {
        get => string.IsNullOrWhiteSpace(_path) ? "/sigger" : _path;
        set => _path = value;
    }

    /// <summary>
    /// The path to call the sigger ui
    /// </summary>
    public SiggerUiOptions WithPath(string path)
    {
        _path = path;
        return this;
    }

    /// <summary>
    /// true if the template should not be cached
    /// </summary>
    public SiggerUiOptions WithIgnoreCaching(bool ignore = true)
    {
        IgnoreCaching = ignore;
        return this;
    }
}