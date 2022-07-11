using System.Text.Json.Serialization;

namespace Sigger.Schema;

public class SchemaInfo
{
    /// <summary>Gets or sets the title.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = SchemaConstants.DEFAULT_TITLE;

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the terms of service.</summary>
    [JsonPropertyName("termsOfService")]
    public string? TermsOfService { get; set; }

    /// <summary>Gets or sets the API version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = SchemaConstants.DEFAULT_SCHEMA_VERSION;
}