using System.Reflection;

namespace Sigger.Generator.Parser;

public class SrcMethod
{
    private readonly MethodInfo _methodInfo;

    internal SrcMethod(MethodInfo methodInfo, SrcType returnType)
    {
        _methodInfo = methodInfo;
        Name = methodInfo.Name;
        ReturnType = returnType;
    }

    public MemberInfo MemberInfo => _methodInfo;

    public SrcType ReturnType { get; }

    public string Name { get; }

    public IList<ArgumentDeclaration> Arguments { get; } = new List<ArgumentDeclaration>();

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{ReturnType.ExportedType.Name} {Name}({string.Join(", ", Arguments.Select(x => x.ToString()))})";
    }
}