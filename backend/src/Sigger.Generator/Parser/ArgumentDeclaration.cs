namespace Sigger.Generator.Parser;

public class ArgumentDeclaration
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public ArgumentDeclaration(string name, SrcType srcType, int index)
    {
        SrcType = srcType;
        Name = name;
        Index = index;
    }

    public SrcType SrcType { get; }

    public string Name { get; }

    public int Index { get; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{SrcType.ExportedType.Name} {Name}";
    }
}