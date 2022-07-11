using System.Diagnostics.CodeAnalysis;
using Sigger.Generator.Utils;

namespace Sigger.Generator.Parser;

public class SrcHub : SrcClass
{
    private readonly Dictionary<Type, SrcDefBase> _parsedTypes = new();

    public SrcHub(Type baseType) : base(baseType)
    {
    }

    /// <summary>
    /// List of all found types in parsed types
    /// </summary>
    public IEnumerable<SrcClass> Types => _parsedTypes.Values.OfType<SrcClass>();

    /// <summary>
    /// List of all found enum in parsed types
    /// </summary>
    public IEnumerable<SrcEnum> Enums => _parsedTypes.Values.OfType<SrcEnum>();

    /// <summary>
    /// List of all event methods for the hub
    /// </summary>
    public List<SrcEvent> Events { get; } = new();

    /// <summary>
    /// Try get the type information of generic type 
    /// </summary>
    public bool TryGetType<T>([MaybeNullWhen(false)] out SrcDefBase src)
    {
        return TryGetType(typeof(T), out src);
    }

    /// <summary>
    /// true if the type was already added to hub 
    /// </summary>
    public bool ContainsType(Type type)
    {
        return _parsedTypes.ContainsKey(type);
    }

    /// <summary>
    /// Try get the type information of generic type 
    /// </summary>
    private bool TryGetType(Type type, [MaybeNullWhen(false)] out SrcDefBase src)
    {
        return _parsedTypes.TryGetValue(type, out src);
    }

    /// <summary>
    /// Add Type to Hub
    /// </summary>
    public SrcDefBase GetOrCreateSrcClass(Type type)
    {
        if (TryGetType(type, out var c))
            return c;

        if (type.IsEnum)
        {
            var e = new SrcEnum(type);
            _parsedTypes.Add(type, e);
            return e;
        }

        c = new SrcClass(type);
        _parsedTypes.Add(type, c);
        return c;
    }

    /// <summary>
    /// Add Type to Hub
    /// </summary>
    public bool TryGetOrCreateSrcClass(Type type, IReadOnlyList<SrcType>? genericTypes, out SrcDefBase hubType)
    {
        if (TryGetType(type, out var c))
        {
            hubType = c;
            return false;
        }

        if (type.IsEnum)
        {
            hubType = new SrcEnum(type);
            _parsedTypes.Add(type, hubType);
            return true;
        }

        hubType = new SrcClass(type);
        _parsedTypes.Add(type, hubType);
        return true;
    }

    /// <summary>
    /// Add Type to Hub
    /// </summary>
    public bool TryAdd(Type type, IReadOnlyList<SrcType>? genericTypes = null)
    {
        if (TryGetType(type, out var c))
            return false;

        if (type.IsEnum)
        {
            var e = new SrcEnum(type);
            foreach (var val in EnumMetadata.Create(type))
                e.TryAdd(val);

            _parsedTypes.Add(type, e);
            return true;
        }

        c = new SrcClass(type);
        _parsedTypes.Add(type, c);
        return true;
    }
}