using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourismSmartTransportation.Business.Hubs.Mapping
{
    public class ConnectionMapping<T>
    {
        // Key là sẽ lưu id người dùng
        // Value là một danh sách lưu giá trị duy nhất (HashSet)
        // Cover cho trường hợp một tài khoản người dùng login trên nhiều máy khác nhau
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public Dictionary<T, HashSet<string>> DictionaryConnections()
        {
            return _connections;
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public int GetConnectionsCount(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections.Count;
            }

            return 0;
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}