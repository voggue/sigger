using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Sigger.Generator.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Utils;

public class EnumMetadataTests
{
    private readonly ITestOutputHelper _output;

    [Fact]
    public void ShouldParseEnumMetadata()
    {
        EnumMetadata.ResetCaches();
        AttributeCache.ResetCaches();
        var catalog = EnumMetadata.Create(typeof(TestEnum1));

        _output.WriteLine(JsonConvert.SerializeObject(catalog, Formatting.Indented));

        // Check Sort-Order
        Assert.Equal((int)TestEnum1.EValFirst, catalog[0].Value);
        Assert.Equal((int)TestEnum1.EVal1, catalog[1].Value);
        Assert.Equal((int)TestEnum1.EVal2, catalog[2].Value);

        // Text
        Assert.Equal("Enum Name from Description", catalog[0].Name);
        Assert.Equal("Enum Val 1", catalog[1].Name);
        Assert.Equal(TestEnum1.EVal2.ToString(), catalog[2].Name);
        Assert.Equal(TestEnum1.ELast.ToString(), catalog[3].Name);

        // Index
        Assert.Equal(0, catalog[1].Value);
        Assert.Equal(1, catalog[2].Value);
        Assert.Equal(2, catalog[0].Value);
        Assert.Equal(3, catalog[3].Value);

        // Value
        Assert.Equal((int)TestEnum1.EValFirst, catalog[0].Value);
        Assert.Equal((int)TestEnum1.EVal1, catalog[1].Value);
        Assert.Equal((int)TestEnum1.EVal2, catalog[2].Value);

        // Description
        Assert.Equal("Enum Name from Description", catalog[0].Description);
        Assert.Equal("Description of Val 1", catalog[1].Description);
        Assert.Null(catalog[2].Description);
        Assert.Equal("Enum Val without order", catalog[3].Description);
    }

    [Fact]
    public void ShouldParseEnumMetadataFromDescriptionAttribute()
    {
        EnumMetadata.ResetCaches();
        AttributeCache.ResetCaches();
        var catalog = EnumMetadata.Create(typeof(TestEnum2));

        _output.WriteLine(JsonConvert.SerializeObject(catalog, Formatting.Indented));

        // Check Sort-Order
        Assert.Equal((int)TestEnum2.EnumVal1, catalog[0].Value);
        Assert.Equal((int)TestEnum2.EnumVal2, catalog[1].Value);

        // Text
        Assert.Equal(TestEnum2.EnumVal1.ToString(), catalog[0].Name);
        Assert.Equal(TestEnum2.EnumVal2.ToString(), catalog[1].Name);

        // Index
        Assert.Equal(0, catalog[0].Index);
        Assert.Equal(1, catalog[1].Index);

        // Text
        Assert.Equal("Define Name with description attribute", catalog[0].Description);
        Assert.Null(catalog[1].Description);
    }

    [Fact]
    public void ShouldTakeMetadataFromCache()
    {
        EnumMetadata.ResetCaches();
        AttributeCache.ResetCaches();
        var catalog1 = EnumMetadata.Create(typeof(TestEnum1));
        var catalog2 = EnumMetadata.Create(typeof(TestEnum1));
        Assert.Same(catalog1, catalog2);
    }

    private enum TestEnum1
    {
        [Display(Name = "Enum Val 1", Description = "Description of Val 1", Order = 1)]
        EVal1,

        EVal2,

        [Description("Enum Name from Description")] [Display(Order = 0)]
        EValFirst,

        [Display(Description = "Enum Val without order")]
        ELast
    }

    private enum TestEnum2
    {
        [Description("Define Name with description attribute")]
        EnumVal1,
        EnumVal2
    }

    public EnumMetadataTests(ITestOutputHelper output)
    {
        _output = output;
    }
}