using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Sigger.Schema;

/// <summary>
/// Meta-Model method definition
/// </summary>
public class MethodDefinition : IHasExportedName
{
    /// <summary>
    /// The Name of the property
    /// </summary>
    [JsonPropertyName(SchemaConstants.NAME)]
    public string? Name { get; set; }

    [JsonPropertyName(SchemaConstants.EXPORTED_NAME)]
    public string? ExportedName { get; set; }

    [JsonPropertyName(SchemaConstants.CAPTION)]
    public string? Caption { get; set; }

    [JsonPropertyName(SchemaConstants.DESCRIPTION)]
    public string? Description { get; set; }

    /// <summary>
    /// Sort-Order
    /// </summary>
    [JsonPropertyName(SchemaConstants.ORDER)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Order { get; set; }

    /// <summary>
    /// List of all Parameters of the Method
    /// </summary>
    [JsonPropertyName(SchemaConstants.ARGUMENTS)]
    public IImmutableList<MethodArgumentDefinition>? Arguments { get; set; }

    /// <summary>
    /// Return Value of the Method
    /// </summary>
    [JsonPropertyName(SchemaConstants.RETURN_TYPE)]
    public TypeDefinition? ReturnType { get; set; }
}