using Microsoft.AspNetCore.SignalR;

namespace Sigger.Web.GettingStarted.Hubs.Chat;

public interface IChatEvents
{
    Task OnMessageReceived(Message message);
}

public class ChatHub : Hub<IChatEvents>
{
    public async Task<Message> SendMessage(string message)
    {
        // Getting user works only with authentication
        var user = Context.User?.Identity?.Name ?? Context.UserIdentifier ?? Context.ConnectionId;
        
        await Clients.Others.OnMessageReceived(new Message(DateTime.Now, user, message, false));
        return new Message(DateTime.Now, user, message, true);
    }
}

/// <summary>
/// A Chat Message Object
/// </summary>
/// <param name="Time">Timestamp</param>
/// <param name="User">UserInfo</param>
/// <param name="Content">Content of the message</param>
/// <param name="Sent">True if the message was sent, false if the message was received</param>
public record Message(DateTime Time, string User, string Content, bool Sent);