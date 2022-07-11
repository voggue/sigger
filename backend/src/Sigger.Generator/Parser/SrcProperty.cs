using System.Reflection;
using Sigger.Generator.Utils;

namespace Sigger.Generator.Parser;

public class SrcEnumItem
{
    public CatalogItemMetadata EnumMetadata { get; }
    public SrcEnum EnumType { get; }

    public SrcEnumItem(CatalogItemMetadata enumMetadata, SrcEnum enumType)
    {
        EnumMetadata = enumMetadata;
        EnumType = enumType;
    }

    public string Name => EnumMetadata.Name;

    public string? Description => EnumMetadata.Description;
    
    public int? Order => EnumMetadata.Order;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Name}=${EnumMetadata.Value}";
    }
}

public class SrcProperty
{
    public SrcProperty(PropertyInfo propInfo, SrcType propertyType)
    {
        MemberInfo = propInfo;
        Name = propInfo.Name;
        PropertyType = propertyType;
        HasGetter = propInfo.GetMethod != null;
        HasSetter = propInfo.SetMethod != null;
    }

    public SrcProperty(FieldInfo fieldInfo, SrcType propertyType)
    {
        MemberInfo = fieldInfo;
        Name = fieldInfo.Name;
        PropertyType = propertyType;
        HasGetter = false;
        HasSetter = false;
        IsField = true;
    }

    public MemberInfo MemberInfo { get; }

    public SrcType PropertyType { get; }

    public bool IsField { get; }

    public string Name { get; }

    public bool HasGetter { get; }

    public bool HasSetter { get; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        if (IsField)
            return $"{PropertyType} {Name}";

        var suffix = "{ ";
        if (HasGetter) suffix += "get; ";
        if (HasSetter) suffix += "set; ";
        suffix += "}";

        return $"{PropertyType} {Name} {suffix};";
    }
}