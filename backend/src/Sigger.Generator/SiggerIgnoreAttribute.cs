namespace Sigger.Generator;

/// <summary>
/// Allow ignore of properties or methods
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field)]
public class SiggerIgnoreAttribute : Attribute
{
}