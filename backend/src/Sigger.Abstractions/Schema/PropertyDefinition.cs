using System.Text.Json.Serialization;

namespace Sigger.Schema;

/// <summary>
/// Meta-Model property definition
/// </summary>
public class PropertyDefinition
{


    [JsonPropertyName(SchemaConstants.PROPERTY_TYPE)]
    public TypeDefinition? PropertyType { get; set; }


    [JsonPropertyName(SchemaConstants.EXPORTED_NAME)]
    public string? ExportedName { get; set; }

    [JsonPropertyName(SchemaConstants.CAPTION)]
    public string? Caption { get; set; }

    [JsonPropertyName(SchemaConstants.DESCRIPTION)]
    public string? Description { get; set; }

    /// <summary>
    /// The Name of the property
    /// </summary>
    [JsonPropertyName(SchemaConstants.NAME)]
    public string? Name { get; set; }

    /// <summary>
    /// Sort-Order
    /// </summary>
    [JsonPropertyName(SchemaConstants.ORDER)]
    public int? Order { get; set; }
}