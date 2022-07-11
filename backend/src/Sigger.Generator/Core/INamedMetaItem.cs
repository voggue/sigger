namespace Sigger.Generator.Core;

public interface INamedMetaItem
{
    /// <summary>
    /// The Name of the Definition
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The Clr Name of the Definition
    /// </summary>
    string ClrName { get; }
}