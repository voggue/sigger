using System.Reflection;
using Sigger.Attributes;
using Sigger.Core;

namespace Sigger.Generator.Parser;

public class CodeParserOptions
{
    /// <summary>
    /// Registry of all primitive types
    /// </summary>
    public Dictionary<Type, PrimitiveTypeMetaInfo> BuiltInTypes { get; } = new()
    {
        { typeof(byte), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(int), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(long), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(decimal), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(double), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(float), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(short), new PrimitiveTypeMetaInfo(false, TypeFlags.IsNumber) },
        { typeof(bool), new PrimitiveTypeMetaInfo(false, TypeFlags.IsBoolean) },
        { typeof(string), new PrimitiveTypeMetaInfo(true, TypeFlags.IsString) },
        { typeof(Guid), new PrimitiveTypeMetaInfo(false, TypeFlags.IsString) },
        { typeof(DateTime), new PrimitiveTypeMetaInfo(false, TypeFlags.IsString) },
        { typeof(object), new PrimitiveTypeMetaInfo(false, TypeFlags.IsAny) },
        { typeof(byte[]), new PrimitiveTypeMetaInfo(false, TypeFlags.IsBase64) },
    };


    /// <summary>
    /// Registry of all assembies ignored for Type-Generation
    /// </summary>
    public HashSet<Assembly> IgnoredAssemblies { get; } = new();

    /// <summary>
    /// Registry of ignored types
    /// </summary>
    public HashSet<Type> IgnoredTypes { get; } = new();


    /// <summary>
    /// Defines the strategy with which inheritances are to be resolved.
    /// </summary>
    public BaseClassStrategy BaseClassStrategy { get; set; } = BaseClassStrategy.MergeMembers;

    /// <summary>
    /// Add primitive types to types-collection
    /// </summary>
    public bool ExtractPrimitives { get; set; }

    /// <summary>
    /// Adding enums even if they are not defined in the Supported Assemblies
    /// </summary>
    public bool ExtractEnumsFromNonSupportedAssemblies { get; set; } = true;

    /// <summary>
    /// Defines the strategy for hubs with which inheritances are to be resolved.
    /// </summary>
    public BaseClassStrategy BaseClassStrategyForHub { get; set; } = BaseClassStrategy.MergeMembers;

    /// <summary>
    /// Defines the Default Keep-Value Mode for Events
    /// </summary>
    public KeepValueMode DefaultKeepValueMode { get; set; } = KeepValueMode.KeepLastValue;
}