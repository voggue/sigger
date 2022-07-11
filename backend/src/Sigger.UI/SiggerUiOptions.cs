using Microsoft.AspNetCore.Builder;

namespace Sigger.UI;

public class SiggerUiOptions
{
    private readonly WebApplication _builder;
    private string? _path;

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
}