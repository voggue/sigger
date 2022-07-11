namespace Sigger.Registry;

public interface ISiggerTopic<out T> : ISiggerTopic
{   
    T? Data { get; }
    
    /// <summary>
    /// Returns true if the connectionUid is subscribed for the current topic 
    /// </summary>
    bool IsSubscribed(Guid connectionUid);
}