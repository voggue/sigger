using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sigger.Generator.Utils;

public static class EnumMetadata
{
    private static readonly Dictionary<Type, CatalogItemMetadata[]> _enumMetadataCache = new();


    /// <summary>
    /// Reset Caches for Unit-Tests
    /// </summary>
    public static void ResetCaches()
    {
        _enumMetadataCache.Clear();
    }

    /// <summary>
    /// Use Enum Catalog
    /// </summary>
    /// <returns>Catalog with metadata for the current enum type</returns>
    public static CatalogItemMetadata[] Create(Type type)
    {
        if (!type.IsEnum)
            throw new NotSupportedException("Type not Supported for Enum-Catalog " + type);

        return GetOrCreateCatalog(type);
    }

    private static CatalogItemMetadata[] GetOrCreateCatalog(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (!type.IsValueType)
            throw new NotSupportedException("Type not Supported for Enum-Catalog " + type);

        if (_enumMetadataCache.TryGetValue(type, out var catalog))
            return catalog;

        var newCatalog = new List<CatalogItemMetadata>();
        var displayAttributes = AttributeCache.GetEnumAttributes<DisplayAttribute>(type);
        var descriptionAttributes = AttributeCache.GetEnumAttributes<DescriptionAttribute>(type);

        var index = 0;
        foreach (var e in Enum.GetValues(type))
        {
            if (e == null)
                continue;

            var c = new CatalogItemMetadata
            {
                Value = Convert.ToInt32(e),
                ValueText = e.ToString() ?? "#ERR UNNAMED#",
                Index = index
            };

            if (displayAttributes.TryGetValue(e, out var attr) && attr != null)
            {
                c.Name = attr.GetName()
                         ?? (descriptionAttributes.TryGetValue(e, out var d) && !string.IsNullOrEmpty(d?.Description)
                             ? d.Description
                             : e.ToString() ?? type.Name + "_" + index);
                c.Description = attr.GetDescription();
                c.Order = attr.GetOrder();
            }

            if (string.IsNullOrWhiteSpace(c.Description) && descriptionAttributes.TryGetValue(e, out var desc) && desc != null)
                c.Description = desc.Description;

            if (string.IsNullOrWhiteSpace(c.Name))
                c.Name = e.ToString() ?? type.Name + "_" + index;


            index++;
            newCatalog.Add(c);
        }

        catalog = newCatalog.OrderBy(x => x.Order ?? int.MaxValue)
            .ThenBy(x => x.Index)
            .ToArray();

        _enumMetadataCache[type] = catalog;
        return catalog;
    }
}