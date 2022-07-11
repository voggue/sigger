using System.Text.Json.Serialization;

namespace Sigger.Schema;

public class TypeDefinition
{
    // /// <summary>
    // /// The Name of the type
    // /// </summary>
    // [JsonPropertyName(SchemaConstants.DOTNET_NAME)]
    // public virtual string Name => _typeInfo.ExportedType.Name;


    // /// <summary>
    // /// The .net Type of the Definition
    // /// </summary>
    // [JsonPropertyName(SchemaConstants.DOTNET_TYPE)]
    // public string ClrType { get; set; }


    /// <summary>
    /// The .net Type of the Definition used for export
    /// </summary>
    [JsonPropertyName(SchemaConstants.EXPORTED_TYPE)]
    public string? ExportedType { get; set; }

    /// <summary>
    /// Additional information flags of the Type
    /// </summary>
    [JsonPropertyName(SchemaConstants.FLAGS)]
    public string? FlagsCaption { get; set; }

    /// <summary>
    /// Additional information flags of the Type
    /// </summary>
    [JsonPropertyName(SchemaConstants.FLAGS_VALUE)]
    public int FlagsValue { get; set; }

    [JsonPropertyName(SchemaConstants.CAPTION)]
    public string? Caption { get; set; }


    [JsonPropertyName(SchemaConstants.DESCRIPTION)]
    public string? Description { get; set; }

    [JsonPropertyName(SchemaConstants.DICTIONARY_KEY)]
    public TypeDefinition? DictionaryKeyType { get; set; }

    [JsonPropertyName(SchemaConstants.DICTIONARY_VALUE)]
    public TypeDefinition? DictionaryValueType { get; set; }

    [JsonPropertyName(SchemaConstants.ARRAY_ELEMENT_TYPE)]
    public TypeDefinition? ArrayElementType { get; set; }

    [JsonPropertyName(SchemaConstants.GENERIC_TYPES)]
    public TypeDefinition[]? GenericTypes { get; set; }
}