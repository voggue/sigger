using Sigger.Generator.Utils;

namespace Sigger.Generator.Parser;

public abstract class SrcDefBase
{
    protected SrcDefBase(Type clrType)
    {
        ClrType = clrType;
    }

    public string Name => ClrType.Name;
    public Type ClrType { get; }
    public bool IsEnum => ClrType.IsEnum;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return Name;
    }
}

public class SrcEnum : SrcDefBase
{
    public SrcEnum(Type baseType) : base(baseType)
    {
    }

    public IReadOnlyCollection<SrcEnumItem> Items => _items.Values;

    private readonly Dictionary<string, SrcEnumItem> _items = new();

    public bool TryAdd(CatalogItemMetadata meta)
    {
        return _items.TryAdd(meta.Name, new SrcEnumItem(meta, this));
    }
    
}

public class SrcClass : SrcDefBase
{
    public SrcClass(Type baseType, IReadOnlyList<SrcType>? genericTypes = null) : base(baseType)
    {
        GenericTypes = genericTypes ?? new List<SrcType>();
    }

    public List<SrcMethod> Methods { get; } = new();

    public IReadOnlyList<SrcType> GenericTypes { get; }

    public IReadOnlyCollection<SrcProperty> Properties => _properties.Values;

    private readonly Dictionary<string, SrcProperty> _properties = new();

    public bool TryAdd(SrcProperty prop)
    {
        return _properties.TryAdd(prop.Name, prop);
    }

    public SrcProperty? FindProperty(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) =>
        Properties.FirstOrDefault(x => string.Equals(name, x.Name, comparison));

    public SrcMethod? FindMethod(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) =>
        Methods.FirstOrDefault(x => string.Equals(name, x.Name, comparison));

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        if (!ClrType.IsGenericType) return ClrType.Name;

        var suffix = "";
        suffix += "<";
        suffix += string.Join(", ", GenericTypes.Select(x => x.ToString()));
        suffix += ">";

        var idx = ClrType.Name.LastIndexOf("`", StringComparison.Ordinal);
        if (idx > 0)
            return ClrType.Name.Remove(idx) + suffix;

        return ClrType.Name + suffix;
    }
}