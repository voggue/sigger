using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sigger.Generator;
using Sigger.Generator.Core;
using Sigger.Schema;
using Sigger.Test.Parser;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Generator;

public class GenerationCoreTests
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
        Assert.Equal(1, parsed!.Hubs.Count);
        Assert.Equal(3, parsed.Hubs[0].Definitions!.EnumDefinitions!.Count);
        var dayOfWeek = parsed.Hubs[0].Definitions!.EnumDefinitions!.First(x => x.ExportedName!.Equals(nameof(DayOfWeek)));
        // Values überprüfen
        Assert.Equal((int)DayOfWeek.Friday, dayOfWeek.Items!.First(x => x.ClrName!.Equals(nameof(DayOfWeek.Friday))).IntValue);
        Assert.Equal((int)DayOfWeek.Sunday, dayOfWeek.Items!.First(x => x.ClrName!.Equals(nameof(DayOfWeek.Sunday))).IntValue);
        Assert.Equal((int)DayOfWeek.Saturday, dayOfWeek.Items!.First(x => x.ClrName!.Equals(nameof(DayOfWeek.Saturday))).IntValue);
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
        Assert.Equal(1, parsed!.Hubs.Count);
        Assert.Equal(5, parsed.Hubs[0].Definitions!.ClassDefinitions!.Count);
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
        Assert.Equal(1, parsed!.Hubs.Count);
        Assert.Single(parsed.Hubs[0].Methods!);
        Assert.Equal(1, parsed.Hubs[0].Events!.Count);
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

#pragma warning disable CS8618
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestClasses
    {
        public interface IEventsDefinition
        {
            Task OnHandleDataChanged(string text, int value);
        }

        public class TestHub : Hub<IEventsDefinition>
        {
            public string Name { get; set; }

            public IEventsDefinition Events { get; set; }

            public Task<int> DoSomethingAsync(string[] args)
            {
                return Task.FromResult(-1);
            }
        }
    }
#pragma warning restore CS8618
}