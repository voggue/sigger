using System.Collections.Generic;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class PreviewSummary
{
    
    public int TotalRecords { get; set; }
    
    
    public int ValidRecords { get; set; }
    
    
    public int InvalidRecords { get; set; }
    
    
    public int ColumnsCount { get; set; }

    /// <summary>
    /// Eine Auflistung aller Log-Messages
    /// </summary>
    
    public IReadOnlyCollection<string>? LogMessages { get; set; }

    /// <summary>
    /// Anzahl der Records die aus der Input-Datei geladen wurden
    /// </summary>
    
    public int? InputRecordsCount { get; set; }
}