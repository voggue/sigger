using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class WorkbookMetadataRequest
{
    /// <summary>
    /// Die Uuid des Files 
    /// </summary>
    
    public Guid? FileId { get; set; }

    /// <summary>
    /// Filter für den Dateinamen
    /// </summary>
    
    public string? FileFilter { get; set; }

    /// <summary>
    /// Filter für den Sheetnamen
    /// </summary>
    
    public string? SheetFilter { get; set; }
}