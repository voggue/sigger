namespace Sigger.Test.Generator.TestHubs.RealWorld;

/// <summary>
/// Metadaten einer Spalte eines Worksheets
/// </summary>

public class WorksheetColumnMetadata
{
    /// <summary>
    /// Spaltenname des Worksheets
    /// </summary>
    
    public string? Name { get; set; }

    /// <summary>
    /// Index der Spalte
    /// </summary>
    
    public int Index { get; set; }

    /// <summary>
    /// Bezeichnung des Daten-Typs
    /// </summary>
    
    public string? Type { get; set; }
}