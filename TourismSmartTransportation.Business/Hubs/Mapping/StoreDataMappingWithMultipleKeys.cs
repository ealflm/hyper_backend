using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourismSmartTransportation.Business.Hubs.Mapping
{
    public class StoreDataMappingWithMultipleKeys<K, V>
    {
        // Key là sẽ lưu id người dùng
        // Value là một danh sách lưu giá trị duy nhất (HashSet)
        // Cover cho trường hợp một tài khoản người dùng login trên nhiều máy khác nhau
        private readonly Dictionary<K, HashSet<V>> _maps =
            new Dictionary<K, HashSet<V>>();

        public int Count
        {
            get
            {
                return _maps.Count;
            }
        }

        public void Add(K key, V value)
        {
            lock (_maps)
            {
                HashSet<V> values;
                if (!_maps.TryGetValue(key, out values))
                {
                    values = new HashSet<V>();
                    _maps.Add(key, values);
                }

                lock (values)
                {
                    values.Add(value);
                }
            }
        }

        public int GetValueCountWithKey(K key)
        {
            HashSet<V> values;
            if (_maps.TryGetValue(key, out values))
            {
                return values.Count;
            }

            return 0;
        }

        public IEnumerable<V> GetValues(K key)
        {
            HashSet<V> values;
            if (_maps.TryGetValue(key, out values))
            {
                return values;
            }

            return Enumerable.Empty<V>();
        }

        public bool CheckExistedValue(K key, V value)
        {
            HashSet<V> values;
            if (_maps.TryGetValue(key, out values))
            {
                if (values.Contains(value))
                {
                    return true;
                }
            }

            return false;

        }

        public void Remove(K key, V value)
        {
            lock (_maps)
            {
                HashSet<V> values;
                if (!_maps.TryGetValue(key, out values))
                {
                    return;
                }

                lock (values)
                {
                    values.Remove(value);

                    if (values.Count == 0)
                    {
                        _maps.Remove(key);
                    }
                }
            }
        }

        public void RemoveAllValues(K key)
        {
            lock (_maps)
            {
                if (_maps.ContainsKey(key))
                {
                    _maps.Remove(key);
                }
            }
        }

        public void Remove(dynamic value)
        {
            lock (_maps)
            {
                if (_maps.ContainsKey(value))
                {
                    _maps.Remove(value);
                    return;
                }

                foreach (var item in _maps)
                {
                    if (item.Value.Contains(value))
                    {
                        item.Value.Remove(value);
                    }
                }
            }
        }
    }
}