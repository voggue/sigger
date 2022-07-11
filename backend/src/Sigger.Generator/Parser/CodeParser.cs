using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using Sigger.Attributes;
using Sigger.Generator.Utils;

namespace Sigger.Generator.Parser;

public class CodeParser
{
    [Flags]
    private enum ExtractMembersFlags
    {
        Methods = 0x001,
        Properties = 0x002,
        Fields = 0x0004,
        Inherited = 0x0008
    }


    private readonly Dictionary<Type, SrcHub> _parsedHubs = new();

    private readonly CodeParserOptions _options;

    public CodeParser(CodeParserOptions? options = null)
    {
        _options = options ?? new CodeParserOptions();
    }

    public SrcHub Parse(Type type)
    {
        if (_parsedHubs.TryGetValue(type, out var hub))
            return hub;

        hub = new SrcHub(type);
        _parsedHubs.Add(type, hub);

        _options.SupportedAssemblies.Add(type.Assembly);
        ExtractTypeMembers(hub, hub, ExtractMembersFlags.Methods);

        var hubBase = type.GetBaseType(typeof(Hub<>));
        if (hubBase == null) return hub;
        var eventType = hubBase.GetGenericArguments().FirstOrDefault();
        if (eventType == null) return hub;
        ExtractEvents(hub, eventType);

        return hub;
    }

    /// <summary>
    /// List of all found types in parsed types
    /// </summary>
    public IEnumerable<SrcEnum> Enums => _parsedHubs.Values.SelectMany(x => x.Enums);

    /// <summary>
    /// List of all found types in parsed types
    /// </summary>
    public IEnumerable<SrcClass> Types => _parsedHubs.Values.SelectMany(x => x.Types);

    /// <summary>
    /// Try get the type information of generic type 
    /// </summary>
    public bool TryGetType<T>([MaybeNullWhen(false)] out SrcDefBase src)
    {
        foreach (var hub in _parsedHubs.Values)
        {
            if (hub.ClrType == typeof(T))
            {
                src = hub;
                return true;
            }
            
            if (hub.TryGetType<T>(out src))
                return true;
        }

        src = null;
        return false;
    }

    /// <summary>
    /// List of all found types in parsed types
    /// </summary>
    public IEnumerable<SrcHub> Hubs => _parsedHubs.Values;

    private void ExtractTypeMembers(SrcHub srcHub, SrcDefBase parent, ExtractMembersFlags flags)
    {
        // Declared in Base-Type
        var strategy = parent is SrcHub ? _options.BaseClassStrategyForHub : _options.BaseClassStrategy;

        foreach (var m in parent.ClrType.GetMembers())
        {
            // .net core Methods are ignored
            if (m.DeclaringType == typeof(object))
                continue;

            if (m.DeclaringType != null && m.DeclaringType != parent.ClrType)
            {
                switch (strategy)
                {
                    case BaseClassStrategy.IgnoreOverloaded:
                    case BaseClassStrategy.IgnoreBaseType:
                        continue;
                    case BaseClassStrategy.ExtractSeperateTypes:
                    {
                        var baseClass = srcHub.GetOrCreateSrcClass(m.DeclaringType);
                        ExtractTypeMembers(srcHub, baseClass, flags);
                        continue;
                    }
                }
            }

            if (m is MethodInfo mi)
            {
                if (strategy == BaseClassStrategy.IgnoreOverloaded && mi.GetBaseDefinition().DeclaringType != m.DeclaringType)
                    continue;

                if (!flags.HasFlag(ExtractMembersFlags.Properties) && mi.IsSpecialName)
                    continue;

                if (flags.HasFlag(ExtractMembersFlags.Methods))
                    HandleMethod(srcHub, mi, parent);
            }
            else if (m is PropertyInfo pi)
            {
                if (flags.HasFlag(ExtractMembersFlags.Properties))
                    HandleProperty(srcHub, pi, parent);
            }
            else if (m is FieldInfo fi)
            {
                if (flags.HasFlag(ExtractMembersFlags.Fields))
                    HandleField(srcHub, fi, parent);
            }
        }
    }

    private void ExtractEvents(SrcHub srcHub, Type eventType)
    {
        foreach (var mi in eventType.GetMethods())
        {
            var returnType = RegisterType(srcHub, mi.ReturnType, out var isValidType);
            if (!isValidType) return;

            var attr = mi.GetCustomAttribute<KeepCurrentValueAttribute>();
            var srcMethod = new SrcEvent(mi, returnType, attr?.KeepValueMode ?? _options.DefaultKeepValueMode);

            foreach (var arg in mi.GetParameters())
            {
                var argType = RegisterType(srcHub, arg.ParameterType, out isValidType);
                if (!isValidType) return;

                srcMethod.Arguments.Add(new ArgumentDeclaration(arg.Name ?? $"arg{arg.Position}", argType, srcMethod.Arguments.Count));
            }

            srcHub.Events.Add(srcMethod);
        }
    }

    private void HandleField(SrcHub srcHub, FieldInfo pi, SrcDefBase parent)
    {
        if (parent is not SrcClass srcClass) return;
        var propType = RegisterType(srcHub, pi.FieldType, out var isValidType);
        if (!isValidType) return;

        var prop = new SrcProperty(pi, propType);
        srcClass.TryAdd(prop);
    }

    private void HandleProperty(SrcHub srcHub, PropertyInfo pi, SrcDefBase parent)
    {
        if (parent is not SrcClass srcClass) return;
        var propType = RegisterType(srcHub, pi.PropertyType, out var isValidType);
        if (!isValidType) return;

        var prop = new SrcProperty(pi, propType);
        srcClass.TryAdd(prop);
    }

    private void HandleMethod(SrcHub srcHub, MethodInfo mi, SrcDefBase parent)
    {
        if (parent is not SrcClass srcClass) return;
        var returnType = RegisterType(srcHub, mi.ReturnType, out var isValidType);
        if (!isValidType) return;

        var srcMethod = new SrcMethod(mi, returnType);

        foreach (var arg in mi.GetParameters())
        {
            var argType = RegisterType(srcHub, arg.ParameterType, out isValidType);
            if (!isValidType) return;

            srcMethod.Arguments.Add(new ArgumentDeclaration(arg.Name ?? $"arg{arg.Position}", argType, srcMethod.Arguments.Count));
        }

        srcClass.Methods.Add(srcMethod);

        // ResolveTypeRecursive(arg.ParameterType, argType)
        //
        // if (!_handler.OnHandleMethod(srcMethod))
        //     return false;
        //
        // if (!WalkType(methodInfo.ReturnType)) return false;
        // return methodInfo.GetParameters()
        //     .All(arg => WalkType(arg.ParameterType));
    }

    private SrcType RegisterType(SrcHub srcHub, Type type, out bool isValidType)
    {
        var srcType = new SrcType(type);
        isValidType = RegisterSubType(srcHub, type, srcType);
        return srcType;
    }

    /// <summary>
    /// Parse Sub-Type and register in type-repo
    /// </summary>
    /// <param name="srcHub"></param>
    /// <param name="type"></param>
    /// <param name="srcType"></param>
    /// <returns>returns false if the type is ignored</returns>
    private bool RegisterSubType(SrcHub srcHub, Type type, SrcType srcType)
    {
        while (true)
        {
            if (type == typeof(void))
            {
                srcType.Flags |= TypeFlags.IsVoid;
                srcType.ExportedType = typeof(void);
                return true;
            }

            if (type.IsTask())
            {
                srcType.Flags |= TypeFlags.IsTask | TypeFlags.IsVoid;
                srcType.ExportedType = typeof(void);
                return true;
            }

            if (type.IsGenericTask())
            {
                type = type.GenericTypeArguments[0];
                srcType.Flags |= TypeFlags.IsTask;
                srcType.ExportedType = type;
                continue;
            }

            if (type.IsNullable(out var underl))
            {
                srcType.Flags |= TypeFlags.IsNullable;
                type = underl;
                continue;
            }

            if (type.IsDictionary(out var keyType, out var valueType))
            {
                srcType.Flags |= TypeFlags.IsDictionary | TypeFlags.IsNullable;
                srcType.ExportedType = typeof(IDictionary);
                srcType.DictionaryKeyType = RegisterType(srcHub, keyType, out var keyValid);
                srcType.DictionaryValueType = RegisterType(srcHub, valueType, out var valueValid);
                return keyValid && valueValid;
            }

            if (type.IsKindOfArray(out var arrayType))
            {
                srcType.Flags |= TypeFlags.IsArray | TypeFlags.IsNullable;
                srcType.ExportedType = typeof(Array);
                srcType.ArrayDim++;
                srcType.ArrayElementType = RegisterType(srcHub, arrayType, out var elementValid);
                return elementValid;
            }

            if (type.IsGenericType)
            {
                foreach (var generic in type.GenericTypeArguments)
                {
                    var genericType = RegisterType(srcHub, generic, out var genericValid);
                    if (genericValid)
                        srcType.GenericTypes.Add(genericType);
                }
            }

            if (type.IsByRef || type.IsPointer)
            {
                var elementType = type.GetElementType();
                if (elementType != null)
                {
                    srcType.Flags |= TypeFlags.IsByRef;
                    type = elementType;
                    continue;
                }
            }

            // Ignored Types
            if (_options.IgnoredTypes.Contains(type))
                return false;

            if (type.IsEnum)
            {
                srcType.Flags |= TypeFlags.IsEnum;

                if (!_options.ExtractEnumsFromNonSupportedAssemblies && !_options.SupportedAssemblies.Contains(type.Assembly))
                    return false;

                if (srcHub.ContainsType(type)) return true;
                srcHub.TryAdd(type, srcType.GenericTypes);
            }
            else if (_options.BuiltInTypes.TryGetValue(type, out var meta))
            {
                srcType.Flags |= TypeFlags.IsPrimitive;
                if (meta.Nullable) srcType.Flags |= TypeFlags.IsNullable;
                srcType.Flags |= meta.Flags;
                srcType.Primitive = meta;
                if (!_options.ExtractPrimitives) return true;
                srcHub.TryAdd(type, srcType.GenericTypes);
            }
            else
            {
                srcType.Flags |= TypeFlags.IsComplex | TypeFlags.IsNullable;
                if (!srcHub.TryGetOrCreateSrcClass(type, srcType.GenericTypes, out var srcClass)) return true;

                // Type is maybe not supported
                if (!_options.SupportedAssemblies.Contains(type.Assembly))
                    return false;
                ExtractTypeMembers(srcHub, srcClass, ExtractMembersFlags.Properties | ExtractMembersFlags.Fields | ExtractMembersFlags.Inherited);
            }

            return true;
        }
    }
}