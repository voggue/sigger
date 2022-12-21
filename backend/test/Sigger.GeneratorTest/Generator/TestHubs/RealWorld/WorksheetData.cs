using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

/// <summary>
/// Response Daten eines WorksheetsDataRequests
/// </summary>

public class WorksheetData
{

    /// <summary>
    /// Die ID des Sheets
    /// </summary>
    
    public Guid SheetId { get; set; }

    /// <summary>
    /// Der Index der ersten Zeile
    /// </summary>
    
    public int FromRowIndex { get; set; }

    /// <summary>
    /// Die Anzahl der Zeilen die gefunden wurden
    /// </summary>
    
    public int Count { get; set; }

    /// <summary>
    /// Die Daten der Zeilen
    /// </summary>
    
    public WorksheetRowData[] Rows { get; set; } = default!;
}