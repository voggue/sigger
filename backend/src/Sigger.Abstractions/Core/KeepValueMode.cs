namespace Sigger.Core;

/// <summary>
/// Defines the handling of generated Properties
/// </summary>
public enum KeepValueMode
{
    /// <summary>
    /// Discard the value after notification
    /// </summary>
    FireAndForget = 0,

    /// <summary>
    /// Keep last value
    /// </summary>
    KeepLastValue = 1,

    /// <summary>
    /// Keep last value with initial Value
    /// </summary>
    KeepWithSeedValue = 2
}