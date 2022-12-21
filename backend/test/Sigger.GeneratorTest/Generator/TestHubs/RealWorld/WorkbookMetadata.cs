using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

/// <summary>
/// Metadaten eines Workbooks
/// </summary>

public class WorkbookMetadata
{
    /// <summary>
    /// Die aktuelle Session-Id des Workbooks
    /// </summary>
    
    public string? SessionId { get; set; }

    /// <summary>
    /// Die Id des Workbooks
    /// </summary>
    
    public Guid? Uid { get; set; }


    /// <summary>
    /// Der Worksheets
    /// </summary>
    
    public WorksheetMetadata[]? Sheets { get; set; }
}