namespace Sigger.UI;

/// <summary>
/// Controls when the embedded Sigger test UI is reachable.
/// </summary>
public enum SiggerUiVisibility
{
    /// <summary>UI is always mapped if <see cref="StartupExtensions.UseSiggerUi"/> is called.</summary>
    Always,

    /// <summary>UI is only mapped in the Development environment (recommended default).</summary>
    DevelopmentOnly
}
