using System.Collections.Immutable;
using System.Text.Json.Serialization;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sigger.Schema;

public class HubDefinition : IHasExportedName
{
    [JsonPropertyName(SchemaConstants.DOTNET_TYPE)]
    public string? ClrType { get; set; }

    [JsonPropertyName(SchemaConstants.CAPTION)]
    public string? Caption  { get; set; }

    [JsonPropertyName(SchemaConstants.DESCRIPTION)]
    public string? Description  { get; set; }

    [JsonPropertyName(SchemaConstants.NAME)]
    public string? Name { get; set; }

    [JsonPropertyName(SchemaConstants.EXPORTED_NAME)]
    public string? ExportedName { get; set; }

    [JsonPropertyName(SchemaConstants.METHODS)]
    public ImmutableList<MethodDefinition>? Methods { get; set; }

    [JsonPropertyName(SchemaConstants.PROPERTIES)]
    public IImmutableList<PropertyDefinition>? Properties { get; set; }

    [JsonPropertyName(SchemaConstants.EVENTS)]
    public IImmutableList<EventDefinition>? Events { get; set; }

    [JsonPropertyName(SchemaConstants.DEFINITIONS)]
    public TypeDefinitions? Definitions { get; set; } = new ();

}

public class TypeDefinitions
{
    [JsonPropertyName(SchemaConstants.TYPE_DEFINITIONS)]
    public ImmutableList<ClassDefinition>? ClassDefinitions { get; set; }
    
    [JsonPropertyName(SchemaConstants.ENUM_DEFINITIONS)]
    public ImmutableList<EnumDefinition>? EnumDefinitions { get; set; }
}