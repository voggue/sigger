using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local

namespace Sigger.Registry;

public partial class SiggerRepository<TCtx, TTopicCtx>
{
    private class ConnectionRepo
    {
        private readonly SiggerRepository<TCtx, TTopicCtx> _repository;
        private readonly Dictionary<Guid, RepositoryConnectionItem> _connectionUids = new();
        private readonly Dictionary<string, Guid> _connectionSignalRIds = new();

        public ConnectionRepo(SiggerRepository<TCtx, TTopicCtx> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get or creates a connection item
        /// </summary>
        /// <returns>return true if the topic was newly created</returns>
        public bool GetOrAdd(string signalRId, Func<RepositoryConnectionItem> connectionCtxFactory, out RepositoryConnectionItem connection)
        {
            lock (_connectionSignalRIds)
            lock (_connectionUids)
            {
                // does the name already exist?
                if (_connectionSignalRIds.TryGetValue(signalRId, out var uid) && _connectionUids.TryGetValue(uid, out var c))
                {
                    connection = c;
                    return false;
                }

                connection = connectionCtxFactory();
                _connectionSignalRIds[connection.SignalRId] = connection.Uid;
                _connectionUids[connection.Uid] = connection;
                return true;
            }
        }

        public bool TryGet(string signalRId, [MaybeNullWhen(false)] out RepositoryConnectionItem connection)
        {
            lock (_connectionSignalRIds)
            lock (_connectionUids)
            {
                if (_connectionSignalRIds.TryGetValue(signalRId, out var uid))
                    return _connectionUids.TryGetValue(uid, out connection);

                connection = default;
                return false;
            }
        }

        public bool TryGet(Guid uid, [MaybeNullWhen(false)] out RepositoryConnectionItem connection)
        {
            lock (_connectionUids)
            {
                return _connectionUids.TryGetValue(uid, out connection);
            }
        }

        public bool TryRemove(string signalRId, [MaybeNullWhen(false)] out RepositoryConnectionItem connection)
        {
            lock (_connectionSignalRIds)
            lock (_connectionUids)
            {
                connection = default;
                return _connectionSignalRIds.Remove(signalRId, out var uid) && _connectionUids.Remove(uid, out connection);
            }
        }

        public bool TryRemove(Guid uid, [MaybeNullWhen(false)] out RepositoryConnectionItem connection)
        {
            lock (_connectionSignalRIds)
            lock (_connectionUids)
            {
                if (!_connectionUids.Remove(uid, out connection))
                    return false;

                _connectionSignalRIds.Remove(connection.SignalRId);
                return true;
            }
        }

        public bool TryAdd(RepositoryConnectionItem connectionItem)
        {
            lock (_connectionSignalRIds)
            lock (_connectionUids)
            {
                // does the name already exist?
                // add signalR id first to prevent name collision
                return _connectionSignalRIds.TryAdd(connectionItem.SignalRId, connectionItem.Uid) &&
                       _connectionUids.TryAdd(connectionItem.Uid, connectionItem);
            }
        }

        public Guid? this[string signalR]
        {
            get
            {
                lock (_connectionUids)
                {
                    return _connectionSignalRIds.TryGetValue(signalR, out var uid) ? uid : null;
                }
            }
        }
    }

    private class RepositoryConnectionItem : IRepositoryItem
    {
        private readonly SiggerRepository<TCtx, TTopicCtx> _repository;
        private readonly HashSet<Guid> _topics = new();

        public RepositoryConnectionItem(SiggerRepository<TCtx, TTopicCtx> repository, Guid uid, string signalRId, TCtx? contextData)
        {
            _repository = repository;
            SignalRId = signalRId;
            Uid = uid;
            ContextData = contextData;
        }

        public string SignalRId { get; }

        public Guid Uid { get; }


        public TCtx? ContextData { get; set; }

        public IEnumerable<RepositoryTopicItem> Topics => _topics
            .Select(uid => _repository._topicRepo.TryGet(uid, out var topic) ? topic : null)
            .OfType<RepositoryTopicItem>();

        public void AddTopic(Guid topicUid)
        {
            _topics.Add(topicUid);
        }

        public void RemoveTopic(Guid topicUid)
        {
            _topics.Remove(topicUid);
        }
    }

}