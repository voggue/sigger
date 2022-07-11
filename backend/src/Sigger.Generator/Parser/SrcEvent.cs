using System.Reflection;
using Sigger.Attributes;
using Sigger.Core;

namespace Sigger.Generator.Parser;

public class SrcEvent : SrcMethod
{
    internal SrcEvent(MethodInfo methodInfo, SrcType returnType, KeepValueMode keepValueMode)
        : base(methodInfo, returnType)
    {
        KeepValueMode = keepValueMode;
    }

    /// <summary>
    /// Defines the handling of generated Properties
    /// </summary>
    public KeepValueMode KeepValueMode { get; }
}