using System.Diagnostics.CodeAnalysis;

namespace Sigger.Generator.Utils;

public static class TypeExtensions
{
    /// <summary>
    /// Returns true if the specified type is a nullable type.
    /// </summary>
    public static bool IsNullable(this Type t, [MaybeNullWhen(false)] out Type underl)
    {
        underl = Nullable.GetUnderlyingType(t);
        return underl != null;
    }

    /// <summary>
    /// Returns true if the specified type is a nullable type.
    /// </summary>
    public static bool IsNullable(this Type t)
    {
        return Nullable.GetUnderlyingType(t) != null;
    }

    /// <summary>
    /// Returns true if the specified type is a dictionary or list of keyvaluepair type.
    /// </summary>
    public static bool IsDictionary(this Type t,
        [MaybeNullWhen(false)] out Type keyType, [MaybeNullWhen(false)] out Type valueType)
    {
        if (t.IsOfType(typeof(IDictionary<,>)))
        {
            keyType = t.GenericTypeArguments.Length > 0 ? t.GenericTypeArguments[0] : typeof(object);
            valueType = t.GenericTypeArguments.Length > 1 ? t.GenericTypeArguments[1] : typeof(object);
            return true;
        }

        keyType = null;
        valueType = null;
        return false;
    }

    /// <summary>
    /// Returns true if the Type is instance of the base Type
    /// </summary>
    public static Type? GetBaseType(this Type type, Type cmpType)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType == cmpType)
                return baseType;

            if (baseType.IsGenericType)
            {
                var generic = baseType.GetGenericTypeDefinition();
                if (generic == cmpType)
                    return baseType;
            }

            baseType = baseType.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Returns true if the Type is instance of the base Type
    /// </summary>
    public static bool HasBaseClass(this Type type, params Type[] cmpType)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (cmpType.Contains(baseType))
                return true;

            if (baseType.IsGenericType)
            {
                var generic = baseType.GetGenericTypeDefinition();
                if (cmpType.Contains(generic))
                    return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the Type is instance of Task
    /// </summary>
    public static bool IsTask(this Type type)
    {
        return type == typeof(Task);
    }

    /// <summary>
    /// Returns true if the Type is instance of Task&lt;&gt;
    /// </summary>
    public static bool IsGenericTask(this Type type)
    {
        return IsOfType(type, typeof(Task<>));
    }

    /// <summary>
    /// Checks whether the type can be assigned by the specified type
    /// </summary>
    /// <param name="src"></param>
    /// <param name="cmp">Type to compare</param>
    /// <returns></returns>
    public static bool IsOfType(this Type src, Type cmp)
    {
        return CheckType(src) ||
               src.GetInterfaces().Any(CheckType);

        bool CheckType(Type t)
        {
            if (t == cmp) return true;
            if (!t.IsGenericType) return false;
            var generic = t.GetGenericTypeDefinition();
            return generic == cmp;
        }
    }


    /// <summary>
    /// Returns the data of Display-Attribute if available or null if not defined
    /// </summary>
    public static string Format(this Type t, bool includeNamespace = false)
    {
        var name = t.Name;

        if (includeNamespace)
            name = t.Namespace + "." + name;

        return name;
    }

    /// <summary>
    /// Check if the given type is any kind of array. this inkludes also Task&lt;List&lt;&gt;&gt;, Task&lt;type[]?&gt;, Nullable&lt;type[]&gt; aso...
    /// </summary>
    internal static bool IsKindOfArray(this Type t, [MaybeNullWhen(false)] out Type elementType)
    {
        var underl = Nullable.GetUnderlyingType(t) ?? t;
        if (underl == typeof(string))
        {
            elementType = null;
            return false;
        }

        if (underl.IsArray)
        {
            elementType = underl.GetElementType();
            if (elementType == null)
                throw new NotSupportedException("Extract element Type of Array Type " + underl + " not supported");
            return true;
        }

        if (underl.IsOfType(typeof(IEnumerable<>)))
        {
            elementType = underl.GenericTypeArguments[0];
            if (elementType == null)
                throw new NotSupportedException("Extract element Type of List Type " + underl + " not supported");

            return true;
        }

        elementType = null;
        return false;
    }
}