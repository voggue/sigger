namespace Sigger.Registry;

public interface ISiggerTopic : IRepositoryItem
{
    public string TopicName { get; }
    
    public string SignalRGroupName { get; }
}