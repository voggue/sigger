using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Sigger.Generator.Parser;
using Sigger.Generator.Utils;
using Sigger.Schema;

namespace Sigger.Generator;

public class SchemaGenerator
{
    private readonly SchemaGeneratorOptions _options;
    private readonly CodeParser _typeParser;

    public SchemaGenerator(SchemaGeneratorOptions? options = null)
    {
        _options = options ?? new SchemaGeneratorOptions();
        _typeParser = new CodeParser(_options.ParserOptions);
    }

    public SrcHub AddHub(Type type, string path)
    {
        return _typeParser.Parse(type, path);
    }

    public SchemaDocument CreateSchema()
    {
        var doc = new SchemaDocument
        {
            Info =
            {
                Title = _options.Title ?? SchemaConstants.DEFAULT_TITLE,
                Description = _options.Description,
                TermsOfService = _options.TermsOfService,
                Version = _options.Version ?? SchemaConstants.DEFAULT_SCHEMA_VERSION
            }
        };

        foreach (var hub in _typeParser.Hubs)
            doc.Hubs.Add(CreateHub(hub, _options));

        return doc;
    }

    // private static SchemaCatalog? CreateCatalog(SrcClass srcClass, SchemaGeneratorOptions options)
    // {
    //     if (!srcClass.IsEnum) return null;
    //     var items = EnumMetadata.Create(srcClass.ClrType)
    //         .Select(m => CreateCatalogItem(m, options))
    //         .ToImmutableList();
    //
    //     var display = srcClass.ClrType.GetCustomAttribute<DisplayAttribute>();
    //
    //     var name = options.GetFormattedName(srcClass.Name, FormatKind.EnumName);
    //     return new SchemaCatalog
    //     {
    //         Name = name,
    //         ClrName = srcClass.Name,
    //         Items = items,
    //         Caption = display?.GetName() ?? srcClass.Name,
    //         Description = display?.GetDescription(),
    //     };
    // }

    // private static EnumItemDefinition CreateCatalogItem(CatalogItemMetadata metadata, SchemaGeneratorOptions options)
    // {
    //     return new EnumItemDefinition
    //     {
    //         Name = options.GetFormattedName(metadata.ValueText, FormatKind.EnumItemName),
    //         ClrName = metadata.Name,
    //         Description = metadata.Description,
    //         Order = metadata.Order,
    //         IntValue = metadata.Value,
    //         StringValue = metadata.ValueText
    //     };
    // }

    private static HubDefinition CreateHub(SrcHub hub, SchemaGeneratorOptions options)
    {
        var display = hub.ClrType.GetCustomAttribute<DisplayAttribute>();

        var properties = hub.Properties
            .Select(prop => CreateProperty(prop, options))
            .ToImmutableList();

        if (!options.GenerateEmptyCollections && properties.Count == 0)
            properties = null;

        var methods = hub.Methods
            .Select(method => CreateMethod(method, options))
            .ToImmutableList();

        if (!options.GenerateEmptyCollections && methods.Count == 0)
            methods = null;

        var events = hub.Events
            .Select(method => CreateEvent(method, options))
            .ToImmutableList();

        if (!options.GenerateEmptyCollections && events.Count == 0)
            events = null;

        var classes = hub.Types
            .Select(t => CreateClass(t, options))
            .ToImmutableList();

        if (!options.GenerateEmptyCollections && classes.Count == 0)
            classes = null;

        var enums = hub.Enums
            .Select(t => CreateEnum(t, options))
            .ToImmutableList();

        if (!options.GenerateEmptyCollections && enums.Count == 0)
            enums = null;

        TypeDefinitions? definitions = null;
        if (enums != null || classes != null)
            definitions = new TypeDefinitions
            {
                ClassDefinitions = classes,
                EnumDefinitions = enums
            };


        return new HubDefinition
        {
            ClrType = hub.ClrType.Format(true),
            Caption = display?.GetName() ?? hub.Name,
            Description = display?.GetDescription(),
            Path = hub.Path,
            Name = hub.Name,
            ExportedName = options.GetFormattedName(hub.ClrType.Name, FormatKind.HubName),
            Properties = properties,
            Methods = methods,
            Definitions = definitions,
            Events = events
        };
    }

    private static EventDefinition CreateEvent(SrcEvent ev, SchemaGeneratorOptions options)
    {
        var display = ev.MemberInfo.GetCustomAttribute<DisplayAttribute>();
        return new EventDefinition
        {
            ReturnType = CreateType(ev.ReturnType, options),
            Arguments = ev.Arguments.Select(arg => CreateMethodArgument(arg, options)).ToImmutableList(),
            Name = ev.Name,
            ExportedName = options.GetFormattedName(ev.Name, FormatKind.MethodName),
            Caption = display?.GetName() ?? ev.Name,
            Description = display?.GetDescription(),
            Order = display?.GetOrder(),
            KeepValueMode = ev.KeepValueMode
        };
    }


    private static MethodArgumentDefinition CreateMethodArgument(ArgumentDeclaration arg, SchemaGeneratorOptions options)
    {
        return new MethodArgumentDefinition
        {
            ExportedName = options.GetFormattedName(arg.Name, FormatKind.MethodArgumentName),
            Caption = arg.SrcType.ExportedType?.Name,
            ClrName = arg.Name,
            Order = arg.Index,
            Type = CreateType(arg.SrcType, options),
        };
    }

    private static EnumDefinition CreateEnum(SrcEnum srcEnum, SchemaGeneratorOptions options)
    {
        var display = srcEnum.ClrType.GetCustomAttribute<DisplayAttribute>();


        return new EnumDefinition
        {
            Items = srcEnum.Items
                .Select(x => CreateEnumItem(x, options))
                .ToImmutableList(),


            ExportedName = options.GetFormattedName(srcEnum.ClrType.Name, FormatKind.EnumName),
            ClrType = srcEnum.ClrType.Format(true),
            Caption = display?.GetName() ?? srcEnum.ClrType.Name,
            Description = display?.GetDescription(),
            Order = display?.GetOrder()
        };
    }

    private static ClassDefinition CreateClass(SrcClass srcClass, SchemaGeneratorOptions options)
    {
        var display = srcClass.ClrType.GetCustomAttribute<DisplayAttribute>();

        return new ClassDefinition
        {
            Properties = srcClass.Properties
                .Select(x => CreateProperty(x, options))
                .ToImmutableList(),

            ExportedName = options.GetFormattedName(srcClass.ClrType.Name, FormatKind.ClassName),
            ClrType = srcClass.ClrType.Format(true),
            Caption = display?.GetName() ?? srcClass.ClrType.Name,
            Description = display?.GetDescription(),
            Order = display?.GetOrder()
        };
    }

    private static EnumItemDefinition CreateEnumItem(SrcEnumItem item, SchemaGeneratorOptions options)
    {
        return new EnumItemDefinition
        {
            Caption = item.Name,
            Description = item.Description,
            Order = item.Order,
            IntValue = item.EnumMetadata.Value,
            StringValue = item.EnumMetadata.ValueText,
            ClrName = item.EnumMetadata.ValueText,
            ExportedName = options.GetFormattedName(item.EnumMetadata.ValueText, FormatKind.EnumItemName),
            ExportedValue = options.GetFormattedName(item.EnumMetadata.ValueText, FormatKind.EnumItemValue),
        };
    }

    private static PropertyDefinition CreateProperty(SrcProperty prop, SchemaGeneratorOptions options)
    {
        var display = prop.MemberInfo.GetCustomAttribute<DisplayAttribute>();

        return new PropertyDefinition
        {
            Caption = display?.GetName() ?? prop.Name,
            Description = display?.GetDescription(),
            Order = display?.GetOrder(),
            Name = prop.Name,
            ExportedName = options.GetFormattedName(prop.Name, FormatKind.PropertyName),
            PropertyType = CreateType(prop.PropertyType, options),
        };
    }

    private static MethodDefinition CreateMethod(SrcMethod method, SchemaGeneratorOptions options)
    {
        var display = method.MemberInfo.GetCustomAttribute<DisplayAttribute>();
        return new MethodDefinition
        {
            ReturnType = CreateType(method.ReturnType, options),
            Arguments = method.Arguments.Select(arg => CreateMethodArgument(arg, options)).ToImmutableList(),
            Name = method.Name,
            ExportedName = options.GetFormattedName(method.Name, FormatKind.MethodName),
            Caption = display?.GetName() ?? method.Name,
            Description = display?.GetDescription(),
            Order = display?.GetOrder(),
        };
    }


    private static TypeDefinition CreateType(SrcType typeInfo, SchemaGeneratorOptions options)
    {
        var display = typeInfo.ClrBaseType.GetCustomAttribute<DisplayAttribute>();

        var def = new TypeDefinition();

        if (typeInfo.DictionaryKeyType != null)
            def.DictionaryKeyType = CreateType(typeInfo.DictionaryKeyType, options);

        if (typeInfo.DictionaryValueType != null)
            def.DictionaryValueType = CreateType(typeInfo.DictionaryValueType, options);

        if (typeInfo.ArrayElementType != null)
            def.ArrayElementType = CreateType(typeInfo.ArrayElementType, options);

        if (typeInfo.GenericTypes.Count > 0)
            def.GenericTypes = typeInfo.GenericTypes.Select(g => CreateType(g, options)).ToArray();

        // ClrType = _typeInfo.ToString();
        def.ExportedType = options.GetFormattedTypeName(typeInfo, TypeKind.TypeName);
        def.FlagsCaption = typeInfo.Flags.ToString();
        def.FlagsValue = (int)typeInfo.Flags;
        def.Caption = display?.GetName() ?? typeInfo.ExportedType.Name;
        def.Description = display?.GetDescription();

        return def;
    }
}