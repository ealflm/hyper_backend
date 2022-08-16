using System;
using System.Collections.Generic;

namespace TourismSmartTransportation.Business.Hubs
{
    public class SearchMapping
    {
        private readonly Dictionary<string, List<DataHubModel>> _dictionary =
            new Dictionary<string, List<DataHubModel>>();

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public Dictionary<string, List<DataHubModel>> GetItems()
        {
            return _dictionary;
        }

        public void UpdateDictionaryValue(string key, List<DataHubModel> list)
        {
            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = list;
                }
            }
        }

        public void Add(string key, DataHubModel value)
        {
            lock (_dictionary)
            {
                List<DataHubModel> data;
                if (!_dictionary.TryGetValue(key, out data))
                {
                    data = new List<DataHubModel>();
                    _dictionary.Add(key, data);
                }

                lock (data)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i].Id.Equals(value.Id))
                        {
                            return;
                        }
                    }

                    data.Add(value);
                }
            }
        }

        public List<DataHubModel> GetValues(string key)
        {
            List<DataHubModel> values;
            lock (_dictionary)
            {
                if (_dictionary.TryGetValue(key, out values))
                {
                    return values;
                }
            }
            return values;
        }

        public void Remove(string key)
        {
            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary.Remove(key);
                }
            }
        }
    }
}