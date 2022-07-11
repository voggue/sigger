using System;
using System.Linq;
using Sigger.Generator.Parser;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Parser;

public abstract class ParserTestsBase : ITestOutputHelper
{
    private readonly ITestOutputHelper _output;

    protected ParserTestsBase(ITestOutputHelper output)
    {
        _output = output;
    }


    /// <summary>Adds a line of text to the output.</summary>
    /// <param name="message">The message</param>
    public void WriteLine(string message)
    {
        _output.WriteLine(message);
    }

    /// <summary>Formats a line of text and adds it to the output.</summary>
    /// <param name="format">The message format</param>
    /// <param name="args">The format arguments</param>
    public void WriteLine(string format, params object[] args)
    {
        _output.WriteLine(format, args);
    }
    
    protected void TraceVisitorResult(CodeParser typeParser)
    {
        WriteLine("Hubs:");
        foreach (var hub in typeParser.Hubs)
        {
            WriteLine("  == Hub: {0}", hub);
            WriteLine("     Methods");
            foreach (var method in hub.Methods)
            {
                _output.WriteLine("       - {0}", method);
            }
        }

        WriteLine("");
        WriteLine("Types:");
        foreach (var type in typeParser.Types)
        {
            WriteLine("  == Type: {0}", type);
            if (type.Methods.Any())
            {
                WriteLine("     Methods:");
                foreach (var method in type.Methods)
                {
                    WriteLine("       - {0}", method);
                }
            }

            if (type.Properties.Any())
            {
                WriteLine("     Properties:");
                foreach (var prop in type.Properties)
                {
                    WriteLine("         - {0}", prop);
                }
            }
        }
        
        foreach (var type in typeParser.Enums)
        {
            WriteLine("  == Enum: {0}", type);
            if (!type.Items.Any()) continue;
            WriteLine("     Items:");
            foreach (var i in type.Items)
            {
                WriteLine("       - {0}", i);
            }
        }
    }
    
    
    protected void AssertMethod(SrcMethod? m, Type exportedReturnType, TypeFlags returnTypeFlags = TypeFlags.None)
    {
        Assert.NotNull(m);
        if (m == null) return;
        AssertType(m.ReturnType, exportedReturnType, returnTypeFlags);
    }

    protected void AssertType(SrcType? type, Type exportedType, TypeFlags requiredFlags)
    {
        Assert.Equal(exportedType, type?.ExportedType);

        if (requiredFlags == TypeFlags.None) return;
        foreach (var availableFlag in Enum.GetValues<TypeFlags>()
                     .Where(x => x != TypeFlags.None && requiredFlags.HasFlag(x)))
        {
            Assert.True(type.Flags.HasFlag(availableFlag), $"required Flag {availableFlag} is not set for Type '{type}'.  Flags: {type.Flags}");
        }
    }
}