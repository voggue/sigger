using System;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class PreviewSessionState
{
    /// <summary>
    /// Anzahl der aktuell gewählten Import-Records die importiert werden sollen
    /// </summary>

    public int SelectedRecords { get; set;}


    /// <summary>
    /// Gibt true zurück, wenn mindestens eine Spalte Sub-Spalten definiert hat
    /// </summary>

    public bool HasMultilineHeader { get; set;}

    /// <summary>
    /// Zusammenfassung der Preview-Daten
    /// </summary>

    public PreviewSummary? Summary { get; set; }

    /// <summary>
    /// Wenn bei der Ausführung eine Exception aufgetreten ist, wird diese hier gespeichert
    /// </summary>

    public PreviewInfoMessage? ExecutionError { get; set; }

    /// <summary>
    /// Wird aktuell ein Dialog angzeigt, dann werden die Daten des Dialogs in diesem Property gespeichert
    /// </summary>

    public DialogData? Dialog { get; }


    /// <summary>
    /// True wenn die Preview gerade erstellt wird
    /// </summary>

    public bool IsLoading { get; }

    /// <summary>
    /// Die Letzte Progress-Nachricht
    /// </summary>

    public string? LastMessage { get; }

    /// <summary>
    /// Die Id des geladenen Importers
    /// </summary>

    public int? ImporterId { get; }

    /// <summary>
    /// Der Name des geladenen Importers
    /// </summary>

    public string? ImporterName { get; }

    /// <summary>
    /// Die ID des geladenen Files
    /// </summary>

    public long? FileId { get; }

    /// <summary>
    /// Die ID de Session
    /// </summary>

    public string SessionId { get; }

    /// <summary>
    /// Anzahl aller Clients die mit der Session verbunden sind
    /// </summary>

    public int ConnectedClients { get; }

    /// <summary>
    /// Zeitpunkt wann die Session erstellt wurde
    /// </summary>

    public DateTime Created { get; }
}