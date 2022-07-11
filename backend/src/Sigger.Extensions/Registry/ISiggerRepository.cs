using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Registry;

public interface ISiggerRepository<TCtx, TTopicCtx>
    where TCtx : class
    where TTopicCtx : class
{
    /// <summary>
    /// Register context data for connection
    /// </summary>
    bool Register(Hub hub, TCtx user, Guid? uid = null);

    /// <summary>
    /// Unregister context data for connection
    /// </summary>
    Task<TCtx?> Unregister(Hub hub);

    /// <summary>
    /// Get or create a topic
    /// </summary>
    /// <returns>return true if the topic was exists</returns>
    bool TryGetTopic(Guid roomId, [MaybeNullWhen(false)] out ISiggerTopic<TTopicCtx> topic);

    /// <summary>
    /// Get or create a topic
    /// </summary>
    /// <returns>return true if the topic was newly created</returns>
    bool GetOrCreateTopic(Hub hub, string topicName, Func<Guid, string, TTopicCtx> factory, out ISiggerTopic<TTopicCtx> topic);

    /// <summary>
    /// Register a connection for a topic
    /// </summary>
    Task<bool> RegisterForTopic(Hub hub, Guid topicUid);

    /// <summary>
    /// Unregister a connection for a topic
    /// </summary>
    Task<bool> UnregisterFromTopic(Hub hub, Guid topicUid);

    /// <summary>
    /// delete a topic
    /// </summary>
    Task<TTopicCtx?> DeleteTopic(Hub hub, Guid topicUid);

    /// <summary>
    /// get a list of all available topics
    /// </summary>
    Task<IReadOnlyCollection<ISiggerTopic<TTopicCtx>>> GetTopics();

    /// <summary>
    /// Get UID for registred context
    /// </summary>
    Guid? GetUid(Hub hub);

    /// <summary>
    /// Get User-Info for current connection
    /// </summary>
    bool TryGetUser(Hub hub, [MaybeNullWhen(false)] out TCtx user);

    /// <summary>
    /// Get all registered connections for a topic
    /// </summary>
    TCtx[] GetTopicMembers(Guid topicUid);
}

public interface ISiggerRepository<TCtx> : ISiggerRepository<TCtx, SiggerTopic>
    where TCtx : class
{
}