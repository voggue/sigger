namespace Sigger.Generator.Parser;

/// <summary>
/// Defines the strategy with which inheritances are to be resolved
/// </summary>
public enum BaseClassStrategy
{
    /// <summary>
    /// Each base class is extracted as a separate type.
    /// This enables the use of base classes in the generated code.
    /// </summary>
    ExtractSeperateTypes = 0,

    /// <summary>
    /// Each member of the base class is included in the inherited generated type.
    /// Parent classes are thus ignored if they are not used in the code.
    /// </summary>
    MergeMembers = 1,

    /// <summary>
    /// Base-types will be ignored
    /// </summary>
    IgnoreBaseType = 2,
    
    /// <summary>
    /// Base-types and overloaded will be ignored
    /// </summary>
    IgnoreOverloaded = 3
}