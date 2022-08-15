using System.Collections.Generic;
using System.Linq;

namespace TourismSmartTransportation.Business.Hubs
{
    public class Mapping<T>
    {
        private readonly Dictionary<string, T> _map =
            new Dictionary<string, T>();

        public int Count
        {
            get
            {
                return _map.Count;
            }
        }

        public Dictionary<string, T> GetItems()
        {
            return _map;
        }

        public void Add(string key, T value)
        {
            lock (_map)
            {
                if (!_map.ContainsKey(key))
                {
                    _map.Add(key, value);
                    return;
                }

                _map[key] = value;
            }
        }
        public T GetValue(string key)
        {
            T value;
            lock (_map)
            {
                if (_map.TryGetValue(key, out value))
                {
                    return value;
                }
            }

            return value;
        }
        public void Remove(string key)
        {
            lock (_map)
            {
                if (_map.ContainsKey(key))
                {
                    _map.Remove(key);
                }
            }
        }
    }
}