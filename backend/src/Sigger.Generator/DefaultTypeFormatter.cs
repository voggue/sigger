using System.Text;
using Sigger.Generator.Parser;
using Sigger.Generator.Utils;

namespace Sigger.Generator;

internal class DefaultTypeFormatter : ITypeFormatter
{
    public static ITypeFormatter Inst { get; } = new DefaultTypeFormatter();

    private static string FirstLetterLowerCase(string name)
    {
        if (name.Length == 0) return name;
        // If all characters are upper case then they are abbreviations of some kind Depending on the setting,
        // the entire name is converted to lower case letters.
        if (name.All(char.IsUpper))
            return name.ToLowerInvariant();

        return char.ToLowerInvariant(name[0]) + name[1..];
    }

    private static string FirstLetterUpperCase(string name)
    {
        if (name.Length == 0) return name;
        return char.ToUpperInvariant(name[0]) + name[1..];
    }

    private static string RemoveAsyncSuffix(string name)
    {
        return name.EndsWith("async", StringComparison.OrdinalIgnoreCase) ? name.Remove(name.Length - 5) : name;
    }

    private static string CleanupName(string name)
    {
        var idx = name.LastIndexOf("`", StringComparison.Ordinal);
        if (idx > 0) name = name.Remove(idx);
        return name;
    }

    private static string AllUpperCase(string name)
    {
        switch (name.Length)
        {
            case 0:
                return name;
            case < 2:
                return name.ToUpperInvariant();
        }

        var sb = new StringBuilder();
        sb.Append(char.ToUpperInvariant(name[0]));
        var prev = name[0];
        for (var i = 1; i < name.Length; ++i)
        {
            var c = name[i];

            if (char.IsUpper(c) && !char.IsUpper(prev))
                sb.Append('_');

            if (!char.IsLetterOrDigit(c)) continue;

            sb.Append(char.ToUpperInvariant(c));
            prev = c;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formatieren eines Namenes
    /// </summary>
    public string GetFormattedName(SchemaGeneratorOptions options, string name, FormatKind kind)
    {
        switch (kind)
        {
            case FormatKind.TypeDeclaration:
            case FormatKind.TypeName:
                if (typeof(void).FullName?.Equals(name) == true || typeof(void).Name.Equals(name))
                    return "void";
                return FirstLetterLowerCase(name);
            case FormatKind.MethodName:
                name = FirstLetterLowerCase(name);
                return RemoveAsyncSuffix(name);
            case FormatKind.EnumItemName:
                return AllUpperCase(name);
            case FormatKind.EnumItemValue:
                return name;
            case FormatKind.EnumName:
                return FirstLetterUpperCase(name);
            case FormatKind.PropertyName:
                return FirstLetterLowerCase(name);
            case FormatKind.ClassName:
                return FirstLetterUpperCase(name);
            case FormatKind.HubName:
                return FirstLetterUpperCase(name);
            case FormatKind.MethodArgumentName:
                return FirstLetterLowerCase(name);
            default:
                throw new NotImplementedException();
        }
    }

    public string GetFormattedTypeName(SchemaGeneratorOptions options, SrcType type, TypeKind kind)
    {
        if (type.Flags.HasFlag(TypeFlags.IsDictionary) && type.DictionaryKeyType != null && type.DictionaryValueType != null)
        {
            var key = GetFormattedTypeName(options, type.DictionaryKeyType, kind);
            var value = GetFormattedTypeName(options, type.DictionaryValueType, kind);
            return "{[key: " + key + "]: " + value + "}";
        }

        if (type.Flags.HasFlag(TypeFlags.IsArray) && type.ArrayElementType != null)
        {
            var element = GetFormattedTypeName(options, type.ArrayElementType, kind);
            var dim = Math.Min(1, type.ArrayDim);
            for (var x = 0; x < dim; x++)
            {
                if (kind == TypeKind.MemberName)
                    element += "Array";
                else
                    element += "[]";
            }

            return element;
        }

        var name = CleanupName(type.ExportedType.Name);
        if (type.GenericTypes.Count > 0)
        {
            var fmt = "";
            foreach (var generic in type.GenericTypes)
            {
                var genericName = GetFormattedTypeName(options, generic, TypeKind.MemberName);
                fmt += FirstLetterUpperCase(genericName);
            }

            fmt += name;
            return fmt;
        }

        if (options.ParserOptions.BuiltInTypes.TryGetValue(type.ArrayElementType?.ExportedType ?? type.ExportedType, out var m))
        {
            if (m.Flags.HasFlag(TypeFlags.IsNumber))
                return "number";
            if (m.Flags.HasFlag(TypeFlags.IsBoolean))
                return "boolean";
            if (m.Flags.HasFlag(TypeFlags.IsString))
                return "string";
            if (m.Flags.HasFlag(TypeFlags.IsBase64))
                return "base64";
            if (m.Flags.HasFlag(TypeFlags.IsAny))
                return "any";

            return $"{TypePrefix}{FirstLetterLowerCase(name)}{TypeSuffix}";
        }

        var baseType = Nullable.GetUnderlyingType(type.ClrBaseType) ?? type.ClrBaseType;
        if (baseType.IsEnum)
        {
            return GetFormattedName(options, baseType.Name, FormatKind.EnumName);
        }

        switch (kind)
        {
            case TypeKind.MemberName:
                if (typeof(void).FullName?.Equals(name) == true || typeof(void).Name.Equals(name))
                    return "";
                return type.Flags.HasFlag(TypeFlags.IsComplex)
                    ? $"{TypePrefix}{FirstLetterUpperCase(type.ExportedType.Name)}{TypeSuffix}"
                    : $"{TypePrefix}{FirstLetterLowerCase(type.ExportedType.Name)}{TypeSuffix}";

            case TypeKind.TypeName:
                if (typeof(void).FullName?.Equals(name) == true || typeof(void).Name.Equals(name))
                    return "void";

                return type.Flags.HasFlag(TypeFlags.IsComplex)
                    ? $"{TypePrefix}{FirstLetterUpperCase(type.ExportedType.Name)}{TypeSuffix}"
                    : $"{TypePrefix}{FirstLetterLowerCase(type.ExportedType.Name)}{TypeSuffix}";
            default:
                throw new NotImplementedException();
        }
    }

    public string TypePrefix { get; set; } = "";

    public string TypeSuffix { get; set; } = "";
}