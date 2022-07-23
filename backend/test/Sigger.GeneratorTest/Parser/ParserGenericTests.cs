// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Global

using Sigger.Generator.Parser;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Parser;

public partial class ParserGenericTests : ParserTestsBase
{
    [Fact]
    public void ShouldExtractBaseClass()
    {
        var options = new CodeParserOptions
        {
            ExtractPrimitives = true
        };
        var typeParser = new CodeParser(options);
        typeParser.Parse(typeof(HubForGenericTests), "/test");
        TraceVisitorResult(typeParser);
        Assert.True(typeParser.TryGetType<SimpleClass>(out _));
        Assert.True(typeParser.TryGetType<NestedGenericClass<string[]>>(out _));
        Assert.True(typeParser.TryGetType<NestedGenericClass<NestedGenericClass<string[]>>>(out _));
        Assert.True(typeParser.TryGetType<NestedGenericClass<NestedGenericClass<NestedGenericClass<string[]>>>>(out _));
    }

    public ParserGenericTests(ITestOutputHelper output) : base(output)
    {
    }
}