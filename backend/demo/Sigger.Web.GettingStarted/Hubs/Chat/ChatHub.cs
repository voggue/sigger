using Microsoft.AspNetCore.SignalR;

namespace Sigger.Web.GettingStarted.Hubs.Chat;

public interface IChatEvents
{
    Task OnMessageReceived(string user, string message);
}

public class ChatHub : Hub<IChatEvents>
{
    public async Task<bool> SendMessage(string message)
    {
        // Getting user works only with authentication
        var user = Context.User?.Identity?.Name ?? Context.UserIdentifier ?? Context.ConnectionId;
        await Clients.All.OnMessageReceived(user, message);
        return true;
    }
}