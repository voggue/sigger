using Sigger.Generator.Parser;

namespace Sigger.Generator;

public class SchemaGeneratorOptions
{
    public CodeParserOptions ParserOptions { get; set; } = new();

    /// <summary>
    /// Name of the declaration for the type e.g.: export interface XXXX
    /// </summary>
    public ITypeFormatter Formatter { get; set; } = DefaultTypeFormatter.Inst;

    /// <summary>
    /// Format a name into exportable name
    /// </summary>
    public string GetFormattedName(string name, FormatKind kind)
    {
        return Formatter.GetFormattedName(this, name, kind);
    }

    /// <summary>
    /// Format a name into exportable name
    /// </summary>
    public string GetFormattedTypeName(SrcType type, TypeKind kind)
    {
        return Formatter.GetFormattedTypeName(this, type, kind);
    }

    /// <summary>
    /// Get Title of Schema
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Get Description of Schema
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Get TermsOfService for Schema
    /// </summary>
    public string? TermsOfService { get; set; }

    /// <summary>
    /// The Version of the Schema
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// true if empty collections are rendered as empty arrays, default false
    /// </summary>
    public bool GenerateEmptyCollections { get; set; } = false;
}