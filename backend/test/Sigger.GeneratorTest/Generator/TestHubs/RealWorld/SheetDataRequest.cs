using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class SheetDataRequest
{
    /// <summary>
    /// Die ID des Sheets
    /// </summary>
    
    public Guid? SheetId { get; set; }

    /// <summary>
    /// Der Index der ersten Zeile, die zurückgegeben werden soll
    /// </summary>
    
    public int FromRowIndex { get; set; }

    /// <summary>
    /// Ein Filter um nach Daten zu suchen
    /// </summary>
    
    public string? Filter { get; set; }
    
    /// <summary>
    /// Die Anzahl der Zeilen, die zurückgegeben werden sollen
    /// </summary>
    
    public int Count { get; set; }
    
    /// <summary>
    /// Die Spalte nach der sortiert werden soll
    /// </summary>
    
    public int? SortColumnIndex { get; set; }
    
    /// <summary>
    /// Wenn sortiert werden soll, dann absteigend
    /// </summary>
    
    public bool? SortDesc { get; set; }
}