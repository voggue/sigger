using System;
using System.Linq;
using System.Text.Json;
using Sigger.Generator;
using Sigger.Generator.Parser;
using Sigger.Schema;
using Sigger.Test.Parser;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Generator;

public partial class GenerationCoreTests
{
    [Fact]
    public void ShouldGenerateSimpleDocument()
    {
        // should generate a hub without exception
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(ParserCoreTests.SimpleHub), "/test");

        var doc = generator.CreateSchema();

        WriteLine(doc.ToJson());
    }


    [Fact]
    public void ShouldGenerateEnums()
    {
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(ParserCoreTests.SimpleHub), "/test");

        var doc = generator.CreateSchema();

        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.StrictEqual(1, parsed!.Hubs.Count);
        Assert.StrictEqual(3, parsed.Hubs[0].Definitions!.EnumDefinitions!.Count);
        var dayOfWeek = parsed.Hubs[0].Definitions!.EnumDefinitions!.First(x => x.ExportedName!.Equals(nameof(DayOfWeek)));
        // Values überprüfen
        Assert.StrictEqual((int)DayOfWeek.Friday, dayOfWeek.Items!.First(x => x.ClrName!.Equals(nameof(DayOfWeek.Friday))).IntValue);
        Assert.StrictEqual((int)DayOfWeek.Sunday, dayOfWeek.Items!.First(x => x.ClrName!.Equals(nameof(DayOfWeek.Sunday))).IntValue);
        Assert.StrictEqual((int)DayOfWeek.Saturday, dayOfWeek.Items!.First(x => x.ClrName!.Equals(nameof(DayOfWeek.Saturday))).IntValue);
    }

    [Fact]
    public void ShouldGenerateGenericsInDocument()
    {
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(ParserGenericTests.HubForGenericTests), "/test");

        var doc = generator.CreateSchema();

        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.StrictEqual(1, parsed!.Hubs.Count);
        Assert.StrictEqual(5, parsed.Hubs[0].Definitions!.ClassDefinitions!.Count);
        Assert.Single(parsed.Hubs[0].Definitions!.EnumDefinitions!);
    }

    [Fact]
    public void ShouldGenerateWithEvents()
    {
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(TestClasses.TestHub), "/test");

        var doc = generator.CreateSchema();
        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.StrictEqual(1, parsed!.Hubs.Count);
        Assert.Single(parsed.Hubs[0].Methods!);
        Assert.StrictEqual(1, parsed.Hubs[0].Events!.Count);
    }

    [Fact]
    public void ShouldGenerateEnumTypes()
    {
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(TestClasses.HubWithEnums), "/test");

        var doc = generator.CreateSchema();
        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.StrictEqual(1, parsed!.Hubs.Count);
        Assert.Single(parsed.Hubs[0].Methods!);
        Assert.Single(parsed.Hubs[0].Definitions!.ClassDefinitions!);

        var type = parsed.Hubs[0].Definitions!.ClassDefinitions![0];
        Assert.Equal(nameof(TestClasses.ClassWithEnums), type.ClrType!.Split('.').Last());


        var properties = type.Properties!.ToDictionary(x => x.Caption ?? "");

        var nonNullable = properties[nameof(TestClasses.ClassWithEnums.NonNullableEnum)];
        var nullable = properties[nameof(TestClasses.ClassWithEnums.NullableEnum)];

        Assert.Equal(nameof(UnitTestEnum), nonNullable.PropertyType?.ExportedType);
        Assert.Equal(TypeFlags.IsEnum, (TypeFlags?)nonNullable.PropertyType?.FlagsValue);

        Assert.Equal(nameof(UnitTestEnum), nullable.PropertyType?.ExportedType);
        Assert.Equal(TypeFlags.IsEnum | TypeFlags.IsNullable, (TypeFlags?)nullable.PropertyType?.FlagsValue);
    }

    [Fact]
    public void ShouldGenerateComplexMethodSchemaInfo()
    {
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(TestClasses.ComplexTestHub), "/test");

        var doc = generator.CreateSchema();
        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.StrictEqual(1, parsed!.Hubs.Count);
        Assert.Single(parsed.Hubs[0].Methods!);

        var method = parsed.Hubs[0].Methods![0];
        Assert.StrictEqual(5, method.Arguments!.Count);
        Assert.Equal(nameof(DateTime), method.Arguments[4].Caption);
    }

    [Fact]
    public void ShouldGenerateSchemaInfo()
    {
        var options = new SchemaGeneratorOptions
        {
            Title = "Unit-Test title",
            Description = "Unit-Test description",
            TermsOfService = "MIT Licence",
            Version = "2.0"
        };
        var generator = new SchemaGenerator(options);

        var doc = generator.CreateSchema();

        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.Equal(options.Title, parsed!.Info.Title);
        Assert.Equal(options.Description, parsed.Info.Description);
        Assert.Equal(options.TermsOfService, parsed.Info.TermsOfService);
        Assert.Equal(options.Version, parsed.Info.Version);
        Assert.Equal(SchemaConstants.DEFAULT_SCHEMA_VERSION, parsed.SchemaVersion);
    }

    [Fact]
    public void ShouldGenerateDefaultSchemaInfo()
    {
        var generator = new SchemaGenerator();

        var doc = generator.CreateSchema();

        var json = doc.ToJson();
        WriteLine(json);
        var parsed = JsonSerializer.Deserialize<SchemaDocument>(json);
        Assert.NotNull(parsed);
        Assert.Equal(SchemaConstants.DEFAULT_TITLE, parsed!.Info.Title);
        Assert.Null(parsed.Info.Description);
        Assert.Null(parsed.Info.TermsOfService);
        Assert.Equal(SchemaConstants.DEFAULT_SCHEMA_VERSION, parsed.Info.Version);
        Assert.Equal(SchemaConstants.SPECIFICATION_VERSION, parsed.SchemaVersion);
    }


    private readonly ITestOutputHelper _output;

    public GenerationCoreTests(ITestOutputHelper output)
    {
        _output = output;
    }


    /// <summary>Adds a line of text to the output.</summary>
    /// <param name="message">The message</param>
    private void WriteLine(string message)
    {
        _output.WriteLine(message);
    }
}

public enum UnitTestEnum
{
    Ok,
    Failure
}