using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sigger.Generator;

namespace Sigger.Test.Generator.TestHubs.RealWorld;

public class SessionHub<TEvents> : Hub<TEvents> where TEvents : class
{
    /// <summary>Called when a connection with the hub is terminated.</summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous disconnect.</returns>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Eine neue Preview-Session erstellen
    /// </summary>
    /// <returns>Gibt die ID der Preview-Session zurück</returns>
    [SiggerIgnore]
    public Task<bool> DisposeSession(string sessionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Eine neue Preview-Session erstellen
    /// </summary>
    /// <returns>Gibt die Preview-Session zurück</returns>
    protected Task<ImporterContextSession> InitializeSession(bool attachToSession)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Einen Client zu einer Session-Attachen
    /// </summary>
    /// <param name="sessionId">Session ID an id Attached werden soll</param>
    /// <returns>true wenn die Session erfolgreich attached wurde</returns> 
    public Task<bool> AttachToSession(string sessionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Ermöglicht das Abfragen des aktuellen Status der Session
    /// </summary>
    public Task<PreviewSessionState?> GetSessionState()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Wird aufgerufen wenn ein client die Session verlässt
    /// </summary>
    public Task OnSessionLeft(string sessionId, string connectionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Wird aufgerufen wenn ein client die Session betritt
    /// </summary>
    public Task OnSessionAttached(string sessionId, string connectionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Wird aufgerufen wenn eine Session beendet wird
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public Task OnSessionClosed(string sessionId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Callback, wenn in der Status der Session geändert wurde
    /// </summary>
    public Task OnSessionInfoChanged(string message, string? details)
    {
        throw new NotImplementedException();
    }
}