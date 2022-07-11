using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Sigger.Schema;

public class EnumDefinition : IHasExportedName
{
    /// <summary>
    /// The .net Type of the Definition
    /// </summary>
    [JsonPropertyName(SchemaConstants.DOTNET_TYPE)]
    public string? ClrType { get; set; }

    [JsonPropertyName(SchemaConstants.CAPTION)]
    public string? Caption { get; set; }

    [JsonPropertyName(SchemaConstants.EXPORTED_NAME)]
    public string? ExportedName { get; set; }

    [JsonPropertyName(SchemaConstants.DESCRIPTION)]
    public string? Description { get; set; }

    /// <summary>
    /// Sort-Order
    /// </summary>
    [JsonPropertyName(SchemaConstants.ORDER)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Order { get; set; }

    [JsonPropertyName(SchemaConstants.ITEMS)]
    public ImmutableList<EnumItemDefinition>? Items { get; set; }

}