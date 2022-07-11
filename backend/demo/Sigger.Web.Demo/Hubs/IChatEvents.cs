namespace Sigger.Web.Demo.Hubs;

public interface IChatEvents
{
    Task OnUserLoggedIn(User? user);

    Task OnUserLoggedOut(User user);

    Task OnUserEnteredChatRoom(ChatRoom room, User? user);

    Task OnUserLeftChatRoom(ChatRoom room, User user);

    Task OnChatRoomMessageReceived(Guid roomId, string roomName, User user, string message);

    Task OnMessageReceived(User user, MessageType type, string message);

    Task OnChatRoomsChanged(IEnumerable<Guid> chatRooms);
}