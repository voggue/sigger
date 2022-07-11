using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sigger.Schema;

/// <summary>
/// Meta-Model Definition
/// </summary>
public class SchemaDocument
{
    /// <summary>Gets or sets the SigSpec specification version being used.</summary>
    [JsonPropertyName(SchemaConstants.SPECIFICATION_VERSION_NAME)]
    public string SchemaVersion { get; set; } = SchemaConstants.SPECIFICATION_VERSION;

    /// <summary>Gets or sets the metadata about the API.</summary>
    [JsonPropertyName(SchemaConstants.INFO)]
    public SchemaInfo Info { get; set; } = new();

    [JsonPropertyName(SchemaConstants.HUBS)]
    public IList<HubDefinition> Hubs { get; set; } = new List<HubDefinition>();

    public string ToJson(JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            IncludeFields = false,
            NumberHandling = JsonNumberHandling.Strict
        };
        return JsonSerializer.Serialize(this, options);
    }
}