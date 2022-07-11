using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Registry;

public static class RepositoryExtensions
{
    /// <summary>
    /// Get all registered connections for a topic
    /// </summary>
    public static TCtx[] GetTopicMembers<TCtx, TTopicCtx>(this ISiggerRepository<TCtx, TTopicCtx> repo, ISiggerTopic topic)
        where TCtx : class
        where TTopicCtx : class
    {
        return repo.GetTopicMembers(topic.Uid);
    }

    /// <summary>
    /// Register a connection for a topic
    /// </summary>
    public static Task<bool> RegisterForTopic<TCtx, TTopicCtx>(this ISiggerRepository<TCtx, TTopicCtx> repo, Hub hub, ISiggerTopic topic)
        where TCtx : class
        where TTopicCtx : class
    {
        return repo.RegisterForTopic(hub, topic.Uid);
    }

    /// <summary>
    /// Get or create a topic
    /// </summary>
    /// <returns>return true if the topic was newly created</returns>
    public static bool GetOrCreateTopic<TCtx, TTopicCtx>(this ISiggerRepository<TCtx, TTopicCtx> repo, Hub hub, string topicName,
        out ISiggerTopic<TTopicCtx> topic)
        where TCtx : class
        where TTopicCtx : class
    {
        return repo.GetOrCreateTopic(hub, topicName, (_, _) => Activator.CreateInstance<TTopicCtx>(), out topic);
    }

    /// <summary>
    /// Get the event handler for alle connections for a topic
    /// </summary>
    public static bool TryGetTopicInvoker<TCtx, TTopicCtx, THub>(this ISiggerRepository<TCtx, TTopicCtx> repo, Hub<THub> hub, Guid topicId,
        [MaybeNullWhen(false)] out THub invoker)
        where TCtx : class
        where TTopicCtx : class
        where THub : class
    {
        if (!repo.TryGetTopic(topicId, out var topic))
        {
            invoker = null;
            return false;
        }

        invoker = hub.Clients.Group(topic.SignalRGroupName);
        return true;
    }

    /// <summary>
    /// Get the event handler for alle connections for a topic
    /// </summary>
    public static bool TryGetTopicInvoker<TCtx, TTopicCtx, THub>(this ISiggerRepository<TCtx, TTopicCtx> repo, Hub<THub> hub, ISiggerTopic topic,
        [MaybeNullWhen(false)] out THub invoker)
        where TCtx : class
        where TTopicCtx : class
        where THub : class
    {
        return TryGetTopicInvoker(repo, hub, topic.Uid, out invoker);
    }
}