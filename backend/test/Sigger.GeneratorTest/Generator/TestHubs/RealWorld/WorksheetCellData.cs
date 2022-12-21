namespace Sigger.Test.Generator.TestHubs.RealWorld;


public class WorksheetCellData
{
    /// <summary>
    /// Der Value der Zelle
    /// </summary>
    
    public string? Value { get; set; }

    /// <summary>
    /// Formatierungsoptionen der Zelle
    /// </summary>
    
    public CellFormatting? Formatting { get; set; }

    /// <summary>
    /// Tooltip der Zelle
    /// </summary>
    
    public string? Info { get; set; }
    
    /// <summary>
    /// Bezeichnung des Daten-Typs
    /// </summary>
    
    public string? Type { get; set; }
}