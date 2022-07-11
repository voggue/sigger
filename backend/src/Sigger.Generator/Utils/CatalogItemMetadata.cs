namespace Sigger.Generator.Utils;

/// <summary>
/// Base-Class for Catalog Metadata Items
/// </summary>
public class CatalogItemMetadata
{
    /// <summary>
    /// Value or ID of the Catalog Item
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Text-Value of the Catalog Item
    /// </summary>
    public string ValueText { get; set; } = null!;

    /// <summary>
    /// Caption of the Catalog Item
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Description of the Catalog Item
    /// </summary>
    public string? Description { get; set; } = null!;

    /// <summary>
    /// Order-Number of the Catalog Item
    /// The Default-Value is int.Max 
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// The Index of definition 
    /// </summary>
    public int Index { get; set; }
}