using System;
using System.ComponentModel.DataAnnotations;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public enum CellFormatting
{
    [Display(Name = "Default")] None,

    [Display(Name = "Info-Zelle")] Info,

    [Display(Name = "Zelle mit Warnung")] Warning,

    [Display(Name = "Zelle mit Fehler")] Error,

    [Display(Name = "Hervorhebung 1")] Highlight1,

    [Display(Name = "Hervorhebung 2")] Highlight2,

    [Display(Name = "Hervorhebung 3")] Highlight3,
}

public record PreviewInfoMessage(string Severity, string Message, string? Details);

public record DialogData(Guid Uid, string Message, string? Title, DialogButton[] Buttons);

/// <summary>
/// Konfiguration eines Buttons
/// </summary>
public class DialogButton
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public DialogButton(DialogResult result)
    {
        Caption = result.ToString().ToUpper();
        Result = result;
    }

    /// <summary>
    /// Eine Bezeichnung des Buttons
    /// </summary>
    public string Caption { get; }

    /// <summary>
    /// Das Ergebnis das bei betätigen des Buttons ausgelöst wird
    /// </summary>
    public DialogResult Result { get; }
}

public enum DialogResult : byte
{
    /// <summary>
    /// Callback wenn der Dialog nicht aufgerufen wurde
    /// </summary>
    [Display(Name = "Nicht verwendet")] Unused = 0,

    /// <summary>
    /// OK
    /// </summary>
    [Display(Name = "Details")] MoreInfo = 1,

    /// <summary>
    /// OK
    /// </summary>
    [Display(Name = "Abbrechen")] Cancel = 2,

    /// <summary>
    /// Yes
    /// </summary>
    [Display(Name = "Ja")] Yes = 4,

    /// <summary>
    /// Yes
    /// </summary>
    [Display(Name = "Nein")] No = 5,
}