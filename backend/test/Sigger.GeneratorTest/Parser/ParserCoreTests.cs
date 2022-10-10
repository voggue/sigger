using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Sigger.Generator.Parser;
using Sigger.Test.Parser.SharedModels;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Parser;

public partial class ParserCoreTests : ParserTestsBase
{
    [Fact]
    public void ParseSimpleHub()
    {
        var options = new CodeParserOptions
        {
            // Default
            BaseClassStrategy = BaseClassStrategy.ExtractSeperateTypes
        };

        var typeParser = new CodeParser(options);
        var result = typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);

        Assert.Equal(4, result.Methods.Count);

        var f1 = result.Methods.FirstOrDefault(t => t.Name == nameof(SimpleHub.MyFunc1Async));
        AssertMethod(f1, typeof(SimpleClass), TypeFlags.IsNullable | TypeFlags.IsComplex);

        var f2 = result.Methods.FirstOrDefault(t => t.Name == nameof(SimpleHub.MyEmptyFunc));
        AssertMethod(f2, typeof(void), TypeFlags.IsVoid);

        var f3 = result.Methods.FirstOrDefault(t => t.Name == nameof(SimpleHub.MyDictionaryFuncAsync));
        AssertMethod(f3, typeof(IDictionary), TypeFlags.IsDictionary);
    }

    [Fact]
    public void ShouldExtractBaseClass()
    {
        var options = new CodeParserOptions
        {
            BaseClassStrategy = BaseClassStrategy.ExtractSeperateTypes,
            BaseClassStrategyForHub = BaseClassStrategy.ExtractSeperateTypes
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);
        Assert.True(typeParser.TryGetType<SimpleBaseClass>(out _));
        Assert.True(typeParser.TryGetType<HubBaseClass>(out _));
    }

    [Fact]
    public void ShouldIgnoreBaseClass()
    {
        var options = new CodeParserOptions
        {
            BaseClassStrategyForHub = BaseClassStrategy.IgnoreBaseType,
            BaseClassStrategy = BaseClassStrategy.ExtractSeperateTypes
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);
        Assert.True(typeParser.TryGetType<SimpleBaseClass>(out _));
    }

    [Fact]
    public void ShouldIgnoreOverloadedMethods()
    {
        var options = new CodeParserOptions
        {
            BaseClassStrategyForHub = BaseClassStrategy.IgnoreOverloaded
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHubWithOverload), "/test");
        TraceVisitorResult(typeParser);
        Assert.True(typeParser.TryGetType<SimpleHubWithOverload>(out var c));
        var srcClass = c as SrcClass;
        Assert.NotNull(srcClass);
        Assert.Single(srcClass!.Methods);
        Assert.NotNull(srcClass.Methods.FirstOrDefault(x =>
            x.Name.Equals(nameof(SimpleHubWithOverload.EmptyHubFunctionAsync))));
    }

    [Fact]
    public void ShouldIgnoreEnumsFromNonSupportedAssmeblies()
    {
        var options = new CodeParserOptions
        {
            ExtractEnumsFromNonSupportedAssemblies = false
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);
        Assert.True(typeParser.TryGetType<SimpleClass>(out var simpleClass));
        Assert.Null((simpleClass as SrcClass)?.FindProperty(nameof(SimpleClass.DotNetEnumType)));
        Assert.False(typeParser.TryGetType<DayOfWeek>(out _));
    }

    [Fact]
    public void ShouldIncludeEnumsFromNonSupportedAssmeblies()
    {
        var options = new CodeParserOptions
        {
            ExtractEnumsFromNonSupportedAssemblies = true
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);
        Assert.True(typeParser.TryGetType<SimpleClass>(out var simpleClass));
        Assert.NotNull((simpleClass as SrcClass)?.FindProperty(nameof(SimpleClass.DotNetEnumType)));
        Assert.True(typeParser.TryGetType<DayOfWeek>(out _));
    }

    [Fact]
    public void ShouldInlineBaseClass()
    {
        var options = new CodeParserOptions
        {
            BaseClassStrategy = BaseClassStrategy.MergeMembers
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);
        Assert.Equal(3, typeParser.Types.Count());
        Assert.Equal(3, typeParser.Enums.Count());
        Assert.False(typeParser.TryGetType<SimpleBaseClass>(out _));
    }


    [Fact]
    public void ShouldHaveEnumItems()
    {
        var typeParser = new CodeParser();
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);

        var enums = typeParser.Enums.ToArray();
        Assert.Equal(3, enums.Length);

        var enumOne = enums.First(x => x.Name == nameof(EnumOne));
        Assert.Equal(3, enumOne.Items.Count);

        var dayOfWeek = enums.First(x => x.Name == nameof(DayOfWeek));
        Assert.Equal(7, dayOfWeek.Items.Count);
    }

    [Fact]
    public void ShouldExtractGenerics()
    {
        var typeParser = new CodeParser();
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);

        // SimpleInterface is defined in a generic Parameter of KeyValuePair
        Assert.True(typeParser.TryGetType<ISimpleInterface>(out _), "Should include generic type");

        // Binding-Flags is a Property in ISimpleInterface
        Assert.True(typeParser.TryGetType<BindingFlags>(out _), "Should include members of generic type");
    }


    [Fact]
    public void ShouldInlineBaseClassAndExtractPrimitives()
    {
        var options = new CodeParserOptions
        {
            BaseClassStrategy = BaseClassStrategy.MergeMembers,
            ExtractPrimitives = true
        };

        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(SimpleHub), "/test");
        TraceVisitorResult(typeParser);
        Assert.False(typeParser.TryGetType<SimpleBaseClass>(out _), "Should include base class");
        Assert.True(typeParser.TryGetType<int>(out _), "Should include primitive type int");
        Assert.True(typeParser.TryGetType<string>(out _), "Should include primitive type int");
    }

    [Fact]
    public void ShouldHaveProperDictionaryTypes()
    {
        var typeParser = new CodeParser();
        var result = typeParser.Parse(typeof(SimpleHub), "/test");
        var method = result.FindMethod(nameof(SimpleHub.MyDictionaryFuncAsync));
        Assert.NotNull(method);
        AssertType(method!.ReturnType, typeof(IDictionary), TypeFlags.IsDictionary);
        Assert.Equal(typeof(string), method.ReturnType.DictionaryKeyType!.ExportedType);
        Assert.Equal(typeof(int), method.ReturnType.DictionaryValueType!.ExportedType);
    }

    [Fact]
    public void ShouldHaveProperArrayType()
    {
        var typeParser = new CodeParser();
        typeParser.Parse(typeof(SimpleHub), "/test");
        var simpleClass = typeParser.Types.FirstOrDefault(x => x.Name == nameof(SimpleClass));
        Assert.NotNull(simpleClass);

        var intArray = simpleClass!.FindProperty(nameof(SimpleClass.IntArray));

        AssertType(intArray?.PropertyType, typeof(Array), TypeFlags.IsArray);
        Assert.Equal(1, intArray?.PropertyType.ArrayDim);
        Assert.Equal(typeof(int), intArray?.PropertyType.ArrayElementType!.ExportedType);
    }

    [Fact]
    public void ShouldHaveProperListType()
    {
        var typeParser = new CodeParser();
        typeParser.Parse(typeof(SimpleHub), "/test");
        var simpleClass = typeParser.Types.FirstOrDefault(x => x.Name == nameof(SimpleClass));
        Assert.NotNull(simpleClass);

        var flagList = simpleClass!.Properties.FirstOrDefault(x => x.Name == nameof(SimpleClass.FlagList));

        AssertType(flagList?.PropertyType, typeof(Array), TypeFlags.IsArray);
        Assert.Equal(1, flagList?.PropertyType.ArrayDim);
        Assert.Equal(typeof(bool), flagList?.PropertyType.ArrayElementType!.ExportedType);
    }


    public ParserCoreTests(ITestOutputHelper output) : base(output)
    {
    }
}