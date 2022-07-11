namespace Sigger.Registry;

public class TopicSubscription<TConnectionCtx, TTopicCtx>
    where TConnectionCtx : class
{
    public TConnectionCtx[] Members { get; }

    public TTopicCtx Topic { get; }

    public string TopicName { get; }

    public Guid TopicUid { get; }

    public TopicSubscription(string topicName, Guid topicUid, TTopicCtx topic, TConnectionCtx[] members)
    {
        Topic = topic;
        Members = members;
        TopicName = topicName;
        TopicUid = topicUid;
    }
}