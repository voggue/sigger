using Sigger.Core;

namespace Sigger.Attributes;

/// <summary>
/// For Events to store the last value client-side
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeepCurrentValueAttribute : Attribute
{
    /// <summary>
    /// Defines the handling of generated Properties
    /// </summary>
    public KeepValueMode KeepValueMode { get; }

    public KeepCurrentValueAttribute(KeepValueMode withSeedValue)
    {
        KeepValueMode = withSeedValue;
    }
}