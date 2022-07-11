using Sigger.Generator.Parser;

namespace Sigger.Generator;

public interface ITypeFormatter
{
    /// <summary>
    /// Formatieren eines Namenes
    /// </summary>
    string GetFormattedName(SchemaGeneratorOptions options, string name, FormatKind kind);

    string GetFormattedTypeName(SchemaGeneratorOptions options, SrcType type, TypeKind kind);
}