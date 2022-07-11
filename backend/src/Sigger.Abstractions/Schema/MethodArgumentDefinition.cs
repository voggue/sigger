using System.Text.Json.Serialization;

namespace Sigger.Schema;

/// <summary>
/// Meta-Model property definition
/// </summary>
public class MethodArgumentDefinition : TypeDefinition
{
    /// <summary>
    /// The Name of the property
    /// </summary>
    [JsonPropertyName(SchemaConstants.EXPORTED_NAME)]
    public string ExportedName { get; set; }

    /// <summary>
    /// The Clr Name of the property
    /// </summary>
    [JsonPropertyName(SchemaConstants.DOTNET_NAME)]
    public string ClrName { get; set; }

    /// <summary>
    /// Sort-Order
    /// </summary>
    [JsonPropertyName(SchemaConstants.ORDER)]
    public int Order { get; set; }

    [JsonPropertyName(SchemaConstants.ARGUMENT_TYPE)]
    public TypeDefinition Type { get; set; }
}