using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sigger.Generator.Utils;

public static class AttributeCache
{
    /// <summary>
    /// Reset Caches for Unit-Tests
    /// </summary>
    public static void ResetCaches()
    {
        _enumCache.Clear();
    }


    private static readonly TypedAttributeDictionaryCache _enumCache = new();

    public static Dictionary<object, TAttribute?> GetEnumAttributes<TAttribute>(Type type)
    {
        if (_enumCache.TryGetValue<TAttribute>(type, out var items))
            return items;

        items = new Dictionary<object, TAttribute?>();

        foreach (var e in Enum.GetValues(type))
        {
            var m = type.GetMember(e.ToString() ?? "").FirstOrDefault();
            if (m == null)
                continue;

            var descAttr = (TAttribute?)m.GetCustomAttributes(typeof(TAttribute), false)
                .FirstOrDefault();

            if (items.ContainsKey(e))
                continue;

            items.Add(e, descAttr);
        }

        _enumCache.Add(type, items);
        return items;
    }


    private class TypedAttributeDictionaryCache
    {
        private readonly Dictionary<Tuple<Type, Type>, IDictionary> _cachedTypes = new();

        public void Clear()
        {
            _cachedTypes.Clear();
        }

        public bool TryGetValue<TAttribute>(Type type,
            [MaybeNullWhen(false)] out Dictionary<object, TAttribute?> cachedEnumValues)
        {
            var key = Tuple.Create(type, typeof(TAttribute));
            if (_cachedTypes.TryGetValue(key, out var cached))
            {
                cachedEnumValues = (Dictionary<object, TAttribute?>)cached;
                return true;
            }

            cachedEnumValues = null;
            return false;
        }

        public void Add<TAttribute>(Type enumType, Dictionary<object, TAttribute?> items)
        {
            var key = Tuple.Create(enumType, typeof(TAttribute));
            _cachedTypes[key] = items;
        }
    }
}