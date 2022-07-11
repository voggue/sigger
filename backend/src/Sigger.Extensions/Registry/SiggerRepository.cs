using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Registry;

public class SiggerRepository<TCtx> : SiggerRepository<TCtx, SiggerTopic>
    , ISiggerRepository<TCtx>
    where TCtx : class
{
}

public partial class SiggerRepository<TCtx, TTopicCtx> : ISiggerRepository<TCtx, TTopicCtx>
    where TCtx : class
    where TTopicCtx : class
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly ConnectionRepo _connectionRepo;
    private readonly TopicRepo _topicRepo;

    // ReSharper disable once MemberCanBeProtected.Global
    public SiggerRepository()
    {
        _connectionRepo = new ConnectionRepo(this);
        _topicRepo = new TopicRepo(this);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    public bool Register(Hub hub, TCtx user, Guid? uid = null)
    {
        _lock.Wait();
        try
        {
            // check if the hub has registred currently a user
            if (_connectionRepo.TryGet(hub.Context.ConnectionId, out var ctx))
            {
                ctx.ContextData = user;
                return false;
            }

            uid ??= Guid.NewGuid();
            ctx = new RepositoryConnectionItem(this, uid.Value, hub.Context.ConnectionId, user);
            return _connectionRepo.TryAdd(ctx);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Unregister a user
    /// </summary>
    public async Task<TCtx?> Unregister(Hub hub)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_connectionRepo.TryRemove(hub.Context.ConnectionId, out var connection))
                return null;

            foreach (var topicCtx in connection.Topics)
            {
                // remove topic for connection and vice versa
                topicCtx.RemoveConnection(connection);
                // register topic in signalr group  
                await hub.Groups.RemoveFromGroupAsync(connection.SignalRId, topicCtx.SignalRGroupName);
            }

            return connection.ContextData;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Get or create a topic
    /// </summary>
    /// <returns>return true if the topic was exists</returns>
    public bool TryGetTopic(Guid roomId, [MaybeNullWhen(false)] out ISiggerTopic<TTopicCtx> topic)
    {
        _lock.Wait();
        try
        {
            if (_topicRepo.TryGet(roomId, out var t))
            {
                topic = t;
                return true;
            }

            topic = default;
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }


    /// <summary>
    /// Get or create a topic
    /// </summary>
    /// <returns>return true if the topic was newly created</returns>
    public bool GetOrCreateTopic(Hub hub, string topicName, Func<Guid, string, TTopicCtx> factory, out ISiggerTopic<TTopicCtx> topic)
    {
        var ret = _topicRepo.GetOrAdd(topicName, () =>
            {
                var uid = Guid.NewGuid();
                return new RepositoryTopicItem(this, uid, topicName, factory(uid, topicName));
            },
            out var t);

        topic = t;
        return ret;
    }


    /// <summary>
    /// Register a connection for a topic
    /// </summary>
    public async Task<bool> RegisterForTopic(Hub hub, Guid topicUid)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_connectionRepo.TryGet(hub.Context.ConnectionId, out var connection))
                throw new UnknownConnectionException(hub.Context.ConnectionId, "Try add topic to unregistered connection");

            if (!_topicRepo.TryGet(topicUid, out var topicCtx))
                throw new UnknownTopicException(topicUid);

            // register topic in signalr group  
            await hub.Groups.AddToGroupAsync(connection.SignalRId, topicCtx.SignalRGroupName);

            // register topic for connection and vice versa
            return topicCtx.AddConnection(connection);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Unregister a connection for a topic
    /// </summary>
    public async Task<bool> UnregisterFromTopic(Hub hub, Guid topicUid)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_topicRepo.TryGet(topicUid, out var topicCtx))
                throw new UnknownTopicException(topicUid);

            // the user is not registered for this topic
            if (!_connectionRepo.TryGet(hub.Context.ConnectionId, out var connection))
                return false;

            // register topic in signalr group  
            await hub.Groups.RemoveFromGroupAsync(connection.SignalRId, topicCtx.SignalRGroupName);

            // remove topic for connection and vice versa
            return topicCtx.RemoveConnection(connection);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// deletes a topic
    /// </summary>
    public async Task<TTopicCtx?> DeleteTopic(Hub hub, Guid topicUid)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_topicRepo.TryRemove(topicUid, out var topicCtx))
                return null;

            foreach (var connection in topicCtx.Connections)
            {
                // remove topic for connection and vice versa
                topicCtx.RemoveConnection(connection);
                // register topic in signalr group  
                await hub.Groups.RemoveFromGroupAsync(connection.SignalRId, topicCtx.SignalRGroupName);
            }

            return topicCtx.Data;
        }
        finally
        {
            _lock.Release();
        }
    }


    /// <summary>
    /// get a list of all registred topics
    /// </summary>
    public async Task<IReadOnlyCollection<ISiggerTopic<TTopicCtx>>> GetTopics()
    {
        await _lock.WaitAsync();
        try
        {
            return _topicRepo.Cast<ISiggerTopic<TTopicCtx>>().ToList();
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Get UID for registred context
    /// </summary>
    public Guid? GetUid(Hub hub)
    {
        return _connectionRepo[hub.Context.ConnectionId];
    }

    /// <summary>
    /// Get User-Info for current connection
    /// </summary>
    public bool TryGetUser(Hub hub, [MaybeNullWhen(false)] out TCtx user)
    {
        if (_connectionRepo.TryGet(hub.Context.ConnectionId, out var connection))
        {
            user = connection.ContextData;
            return user != null;
        }

        user = null;
        return false;
    }

    /// <summary>
    /// Get all registered connections for a topic
    /// </summary>
    public TCtx[] GetTopicMembers(Guid topicUid)
    {
        if (!_topicRepo.TryGet(topicUid, out var topicCtx))
            throw new UnknownTopicException(topicUid);

        return topicCtx.GetMembers();
    }
}