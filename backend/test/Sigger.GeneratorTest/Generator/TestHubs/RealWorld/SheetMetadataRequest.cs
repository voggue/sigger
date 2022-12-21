using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class SheetMetadataRequest
{
    /// <summary>
    /// Die Uuid des Files (muss mit dem aktuellen File übereinstimmen) 
    /// </summary>
    
    public Guid? FileId { get; set; }
}