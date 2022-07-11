using System.Collections;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local

namespace Sigger.Registry;

public partial class SiggerRepository<TCtx, TTopicCtx>
{
    private class TopicRepo : IEnumerable<RepositoryTopicItem>
    {
        private readonly SiggerRepository<TCtx, TTopicCtx> _repository;
        private readonly Dictionary<Guid, RepositoryTopicItem> _topicsUids = new();
        private readonly Dictionary<string, Guid> _topicsNames = new();

        public TopicRepo(SiggerRepository<TCtx, TTopicCtx> repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Get or creates a topic
        /// </summary>
        /// <returns>return true if the topic was newly created</returns>
        public bool GetOrAdd(string name, Func<RepositoryTopicItem> topicCtxFactory, out RepositoryTopicItem topic)
        {
            lock (_topicsUids)
            {
                lock (_topicsNames)
                {
                    // does the name already exist?
                    if (_topicsNames.TryGetValue(name, out var uid) && _topicsUids.TryGetValue(uid, out var t))
                    {
                        topic = t;
                        return false;
                    }

                    t = topicCtxFactory();
                    _topicsUids[t.Uid] = t;
                    _topicsNames[t.TopicName] = t.Uid;

                    topic = t;
                    return true;
                }
            }
        }

        public bool TryGet(Guid uid, [MaybeNullWhen(false)] out RepositoryTopicItem topic)
        {
            lock (_topicsUids)
            {
                return _topicsUids.TryGetValue(uid, out topic);
            }
        }

        public bool TryGet(string name, [MaybeNullWhen(false)] out RepositoryTopicItem topic)
        {
            lock (_topicsNames)
            {
                if (_topicsNames.TryGetValue(name, out var uid))
                    return _topicsUids.TryGetValue(uid, out topic);

                topic = default;
                return false;
            }
        }

        public bool TryRemove(string name, [MaybeNullWhen(false)] out RepositoryTopicItem topic)
        {
            lock (_topicsUids)
            {
                lock (_topicsNames)
                {
                    if (_topicsNames.Remove(name, out var uid))
                        return _topicsUids.Remove(uid, out topic);

                    topic = default;
                    return false;
                }
            }
        }

        public bool TryRemove(Guid uid, [MaybeNullWhen(false)] out RepositoryTopicItem topic)
        {
            lock (_topicsUids)
            {
                lock (_topicsNames)
                {
                    if (!_topicsUids.Remove(uid, out topic))
                        return false;

                    _topicsNames.Remove(topic.TopicName);
                    return true;
                }
            }
        }

        public bool TryRemove(RepositoryTopicItem topic)
        {
            lock (_topicsUids)
            {
                lock (_topicsNames)
                {
                    var ret = _topicsUids.Remove(topic.Uid);
                    ret = _topicsNames.Remove(topic.TopicName) && ret;
                    return ret;
                }
            }
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<RepositoryTopicItem> GetEnumerator()
        {
            lock (_topicsUids)
            {
                return _topicsUids.Values.GetEnumerator();
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    private class RepositoryTopicItem : ISiggerTopic<TTopicCtx>
    {
        private readonly SiggerRepository<TCtx, TTopicCtx> _repository;
        private readonly HashSet<Guid> _connections = new();

        public RepositoryTopicItem(SiggerRepository<TCtx, TTopicCtx> repository, Guid uid, string topicName, TTopicCtx? data)
        {
            _repository = repository;
            Uid = uid;
            SignalRGroupName = uid.ToString();
            TopicName = topicName;
            Data = data;
        }

        public string SignalRGroupName { get; }

        public TTopicCtx? Data { get; set; }

        /// <summary>
        /// Returns true if the connectionUid is subscribed for the current topic 
        /// </summary>
        public bool IsSubscribed(Guid connectionUid)
        {
            return _connections.Contains(connectionUid);
        }

        public Guid Uid { get; }

        public string TopicName { get; }

        public IEnumerable<Guid> ConnectionIds => _connections;

        public IEnumerable<RepositoryConnectionItem> Connections =>
            _connections.Select(x => _repository._connectionRepo
                    .TryGet(x, out var c)
                    ? c
                    : null)
                .OfType<RepositoryConnectionItem>();

        public bool AddConnection(RepositoryConnectionItem connection)
        {
            // register topic in connection
            connection.AddTopic(Uid);

            // register connection in topic
            return _connections.Add(connection.Uid);
        }

        public bool RemoveConnection(RepositoryConnectionItem connection)
        {
            // unregister topic in connection
            connection.RemoveTopic(Uid);

            // unregister connection in topic
            if (!_connections.Contains(connection.Uid))
                return false;

            _connections.Remove(connection.Uid);
            return true;
        }

        public TCtx[] GetMembers()
        {
            return _connections
                .Select(conId => _repository._connectionRepo.TryGet(conId, out var c) ? c.ContextData : null)
                .OfType<TCtx>()
                .ToArray();
        }
    }
}