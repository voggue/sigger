using System.Linq;
using Sigger.Generator;
using Sigger.Schema;
using Sigger.Test.Generator.TestHubs.RealWorld;
using Sigger.Test.Parser;
using Xunit;
using Xunit.Abstractions;

namespace Sigger.Test.Generator;

public class GenerationComplexTests
{
    [Fact]
    public void ShouldGenerateComplexHub()
    {
        var options = new SchemaGeneratorOptions();
        var generator = new SchemaGenerator(options);
        generator.AddHub(typeof(SheetViewerHub), "/test");

        var doc = generator.CreateSchema();

        var hub = doc.Hubs.FirstOrDefault();
        Assert.NotNull(hub);
        DumpHubdata(hub!);

        Assert.Equal(10, hub!.Methods!.Count);
        var methodNames = hub.Methods.Select(x => x.Name).ToArray();
        Assert.Contains("OpenWorksheet", methodNames);
        Assert.Contains("OnSessionAttached", methodNames);
        Assert.DoesNotContain("DisposeSession", methodNames);
        
        Assert.Equal(1, hub!.Events!.Count);
        var eventNames = hub.Events.Select(x => x.Name).ToArray();
        Assert.Contains("OnChangeSession", eventNames);

        Assert.Equal(15, hub.Definitions!.ClassDefinitions!.Count);
        var classNames = hub.Definitions!.ClassDefinitions.Select(x => x.Caption).ToArray();
        Assert.Contains("WorksheetColumnMetadata", classNames);
        Assert.Contains("PreviewInfoMessage", classNames);
        Assert.Contains("SessionId", classNames);

        Assert.Equal(2, hub.Definitions!.EnumDefinitions!.Count);
        var enumNames = hub.Definitions!.EnumDefinitions.Select(x => x.Caption).ToArray();
        Assert.Contains("CellFormatting", enumNames);
        Assert.Contains("DialogResult", enumNames);
    }

    private void DumpHubdata(HubDefinition hub)
    {
        var cnt = 1;
        WriteLine("=== Types ===");
        if (hub.Definitions?.ClassDefinitions != null)
            foreach (var clazz in hub.Definitions!.ClassDefinitions!)
                WriteLine($"    {cnt++}c) {clazz.ExportedName!}");

        cnt = 1;
        WriteLine("=== Enums ===");
        if (hub.Definitions?.EnumDefinitions != null)
            foreach (var enumDef in hub.Definitions!.EnumDefinitions!)
                WriteLine($"    {cnt++}e) {enumDef.ExportedName!}");

        cnt = 1;
        WriteLine("=== Methods ===");
        if (hub.Methods != null)
            foreach (var method in hub.Methods!)
                WriteLine($"    {cnt++}m) {method.ExportedName!}");

        cnt = 1;
        WriteLine("=== Events ===");
        if (hub.Events != null)
            foreach (var method in hub.Events!)
                WriteLine($"    {cnt++}v) {method.ExportedName!}");
    }

    private readonly ITestOutputHelper _output;

    public GenerationComplexTests(ITestOutputHelper output)
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