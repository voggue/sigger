using System.Text.Json.Serialization;

namespace Sigger.Schema;

/// <summary>
/// Meta-Model catalog-base definition
/// </summary>
public class EnumItemDefinition : IHasExportedName
{
    /// <summary>
    /// The Clr Name of the Definition
    /// </summary>
    [JsonPropertyName(SchemaConstants.DOTNET_NAME)]
    public string? ClrName { get; set; }

    [JsonPropertyName(SchemaConstants.CAPTION)]
    public string? Caption { get; set; }

    [JsonPropertyName(SchemaConstants.DESCRIPTION)]
    public string? Description { get; set; }

    /// <summary>
    /// Sort-Order
    /// </summary>
    [JsonPropertyName(SchemaConstants.ORDER)]
    public int? Order { get; set; }

    [JsonPropertyName(SchemaConstants.VALUE_INT)]
    public int IntValue { get; set; }

    [JsonPropertyName(SchemaConstants.VALUE_TEXT)]
    public string? StringValue { get; set; }

    [JsonPropertyName(SchemaConstants.EXPORTED_NAME)]
    public string? ExportedName { get; set; }
    
    
    [JsonPropertyName(SchemaConstants.EXPORTED_VALUE)]
    public string? ExportedValue { get; set; }
}