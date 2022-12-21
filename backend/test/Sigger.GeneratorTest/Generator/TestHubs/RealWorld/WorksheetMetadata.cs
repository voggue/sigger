using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

/// <summary>
/// Metadaten eines Worksheets
/// </summary>

public class WorksheetMetadata
{
    /// <summary>
    /// Die Id des Worksheets
    /// </summary>
    
    public Guid? Uid { get; set; }

    /// <summary>
    /// Name des Worksheets
    /// </summary>
    
    public string? Name { get; set; }

    /// <summary>
    /// Name des übergeordneten Files
    /// </summary>
    
    public string? ParentName { get; set; }

    /// <summary>
    /// Der Pfad des übergeordneten Files
    /// </summary>
    
    public string? ParentPath { get; set; }

    /// <summary>
    /// Die Spalten des Worksheets
    /// </summary>
    
    public WorksheetColumnMetadata[]? Columns { get; set; }

    /// <summary>
    /// Die Anzahl der Zeilen im worksheet
    /// </summary>
    
    public int RowCount { get; set; }
}