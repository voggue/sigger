using System.Text.Json.Serialization;
using Sigger.Core;

namespace Sigger.Schema;

/// <summary>
/// Meta-Model event definition
/// </summary>
public class EventDefinition : MethodDefinition
{
    /// <summary>
    /// Defines the handling of generated Properties
    /// </summary>
    [JsonPropertyName(SchemaConstants.KEEP_VALUE_MODE)]
    public KeepValueMode KeepValueMode { get; set; }
}