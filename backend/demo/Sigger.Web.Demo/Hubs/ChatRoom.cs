namespace Sigger.Web.Demo.Hubs;

public class ChatRoom
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public ChatRoom(Guid uid, string name, User[] members)
    {
        Uid = uid;
        Name = name;
        Members = members;
    }

    public Guid Uid { get; }

    public string Name { get; }
    
    public User[] Members { get; }
    
    
}

public class ChatRoomSubscription : ChatRoom
{
    public bool Subscribed { get; }

    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public ChatRoomSubscription(Guid uid, string name, User[] members, bool subscribed) : base(uid, name, members)
    {
        Subscribed = subscribed;
    }
}