namespace Sigger.Generator.Parser;

public class SrcType
{
    public SrcType(Type baseType)
    {
        ClrBaseType = baseType;
        ExportedType = baseType;
    }

    public Type ClrBaseType { get; }

    public Type ExportedType { get; internal set; }

    public int ArrayDim { get; internal set; }

    public TypeFlags Flags { get; internal set; }

    public PrimitiveTypeMetaInfo? Primitive { get; internal set; }

    public SrcType? ArrayElementType { get; internal set; }

    public SrcType? DictionaryKeyType { get; internal set; }

    public SrcType? DictionaryValueType { get; internal set; }

    public List<SrcType> GenericTypes { get; } = new();

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        if (ClrBaseType.IsGenericType)
        {
            var suffix = "";
            suffix += "<";
            suffix += string.Join(", ", GenericTypes.Select(x => x.ToString()));
            suffix += ">";

            var idx = ClrBaseType.Name.LastIndexOf("`", StringComparison.Ordinal);
            if (idx > 0)
                return ClrBaseType.Name.Remove(idx) + suffix;

            return ClrBaseType.Name + suffix;
        }

        if (Flags.HasFlag(TypeFlags.IsArray) && ArrayElementType != null)
            return $"{ArrayElementType.ClrBaseType.Name}[]";

        if (Flags.HasFlag(TypeFlags.IsDictionary) && DictionaryKeyType != null && DictionaryValueType != null)
            return $"Dictionary<{DictionaryKeyType},{DictionaryValueType}>";

        return ClrBaseType.Name;
    }
}