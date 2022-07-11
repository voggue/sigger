namespace Sigger.Registry;

public class UnknownTopicException : Exception
{
    public UnknownTopicException(ISiggerTopic topic) : base($"Topic {topic.TopicName} ({topic.Uid}) not found")
    {
    }

    public UnknownTopicException(Guid uid) : base($"Topic {uid}) not found")
    {
    }
}