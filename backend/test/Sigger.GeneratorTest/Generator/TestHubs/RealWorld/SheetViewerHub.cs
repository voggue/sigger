using System;
using System.Threading.Tasks;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class SheetViewerHub : SessionHub<ISheetViewerEvents>
{
    /// <summary>
    /// Ein Workbook öffnen und die Metadaten des Workbooks zurückgeben
    /// </summary>
    public Task<WorksheetMetadata> OpenWorksheet(string sessionId, SheetMetadataRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Ein Worksheet öffnen und die Metadaten des Worksheets zurückgeben
    /// </summary>
    public Task<WorkbookMetadata> OpenWorkbook(string sessionId, WorkbookMetadataRequest request)
    {
        throw new NotImplementedException();
    }


    public Task<WorksheetData> GetWorksheetData(string sessionId, SheetDataRequest request)
    {
        throw new NotImplementedException();
    }
}