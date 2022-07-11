using Microsoft.AspNetCore.SignalR;

namespace Sigger.Registry;

public class UnknownConnectionException : Exception
{
    public UnknownConnectionException(string connectionId, string message) : base(message)
    {
        ConnectionId = connectionId;
    }

    public UnknownConnectionException(Hub hub, string message) : base(message)
    {
        ConnectionId = hub.Context.ConnectionId;
    }
    
    public string ConnectionId { get; }
}