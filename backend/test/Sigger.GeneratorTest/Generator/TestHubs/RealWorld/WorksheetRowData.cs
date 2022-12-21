namespace Sigger.Test.Generator.TestHubs.RealWorld;

/// <summary>
/// Response Daten eines WorksheetsDataRequests
/// </summary>

public class WorksheetRowData
{
    /// <summary>
    /// Der Index der Zeile
    /// </summary>
    
    public int RowIndex { get; set; }

    /// <summary>
    /// Die Zellen der Zeile
    /// </summary>
    
    public WorksheetCellData[] Cells { get; set; } = default!;
}