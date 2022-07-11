using Duende.IdentityServer.EntityFramework.Entities;
using Microsoft.AspNetCore.SignalR;
using Sigger.Registry;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sigger.Web.Demo.Hubs;

public class ChatHub : Hub<IChatEvents>
{
    private readonly ISiggerRepository<User> _userRepository;

    public ChatHub(ISiggerRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.</returns>
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    /// <summary>Called when a connection with the hub is terminated.</summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous disconnect.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Logout();
    }

    public async Task<bool> SendMessageToChatRoom(Guid chatRoomId, string message)
    {
        if (!_userRepository.TryGetUser(this, out var user))
            return false;

        if (!_userRepository.TryGetTopic(chatRoomId, out var topic))
            return false;

        if (!_userRepository.TryGetTopicInvoker(this, topic, out var invoker))
            return false;

        await invoker.OnChatRoomMessageReceived(topic.Uid, topic.TopicName, user, message);
        return true;
    }

    public async Task<bool> SendBroadcastMessage(string message)
    {
        if (!_userRepository.TryGetUser(this, out var user))
            return false;

        await Clients.All.OnMessageReceived(user, MessageType.Broadcast, message);
        return true;
    }

    public User? WhoAmI()
    {
        return !_userRepository.TryGetUser(this, out var user) ? null : user;
    }

    public async Task<IEnumerable<ChatRoomSubscription>> GetChatRooms()
    {
        _userRepository.TryGetUser(this, out var user);

        var topics = await _userRepository.GetTopics();
        return topics.Select(topic => new ChatRoomSubscription(
            topic.Uid,
            topic.TopicName,
            _userRepository.GetTopicMembers(topic),
            user != null && topic.IsSubscribed(user.Uid)));
    }

    public async Task<User?> Login(string userName, string color)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        // Create dummy user (usually from database)
        var usr = new User(Guid.NewGuid(), userName, color, null, UserRole.User);

        if (_userRepository.Register(this, usr, usr.Uid))
            await Clients.Others.OnUserLoggedIn(usr);
        return usr;
    }

    public async Task<bool> Logout()
    {
        var usr = await _userRepository.Unregister(this);
        if (usr == null)
            return false;

        await Clients.Others.OnUserLoggedOut(usr);
        await Clients.All.OnChatRoomsChanged(Array.Empty<Guid>());
        return true;
    }

    public async Task<ChatRoomSubscription> CreateChatRoom(string roomName)
    {
        if (!_userRepository.TryGetUser(this, out var user))
            throw new UnknownConnectionException(this, "User not logged in");

        var isNew = _userRepository.GetOrCreateTopic(this, roomName, out var topic);

        var room = new ChatRoomSubscription(topic.Uid, topic.TopicName, _userRepository.GetTopicMembers(topic), topic.IsSubscribed(user.Uid));
        if (!isNew) return room;

        await Clients.All.OnChatRoomsChanged(new[] { topic.Uid });
        return room;
    }

    public async Task<ChatRoom> EnterChatRoom(Guid uid)
    {
        if (!_userRepository.TryGetUser(this, out var user))
            throw new UnknownConnectionException(this, "User not logged in");

        if (!_userRepository.TryGetTopic(uid, out var topic))
            throw new UnknownTopicException(uid);

        var room = new ChatRoom(topic.Uid, topic.TopicName, _userRepository.GetTopicMembers(topic));

        if (!await _userRepository.RegisterForTopic(this, topic))
            return room;

        // user is now subscribed to the topic
        await Clients.Others.OnUserEnteredChatRoom(room, user);

        // now let the users know that the user has entered the room
        await Clients.All.OnChatRoomsChanged(new[] { topic.Uid });

        return room;
    }

    public async Task<bool> LeaveChatRoom(Guid roomId)
    {
        if (!_userRepository.TryGetTopic(roomId, out var topic))
            return false;

        if (await _userRepository.UnregisterFromTopic(this, roomId) == false)
            return false;

        if (!_userRepository.TryGetUser(this, out var user))
            return false;

        var room = new ChatRoom(topic.Uid, topic.TopicName, _userRepository.GetTopicMembers(topic));
        await Clients.Others.OnUserLeftChatRoom(room, user);

        return true;
    }
}