using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.Hubs.Models;

namespace TourismSmartTransportation.Business.Hubs.Mapping
{
    public class AutoCancelMapping
    {
        private Dictionary<string, Tuple<TransferHubModel, string>> _dicStore = new Dictionary<string, Tuple<TransferHubModel, string>>();

        public int Count
        {
            get
            {
                return _dicStore.Count;
            }
        }

        public Dictionary<string, Tuple<TransferHubModel, string>> Store
        {
            get
            {
                return _dicStore;
            }
        }

        public void Add(string key, Tuple<TransferHubModel, string> value)
        {
            lock (_dicStore)
            {
                if (_dicStore.ContainsKey(key))
                {
                    _dicStore[key] = value;
                    return;
                }

                _dicStore.Add(key, value);
            }
        }

        public Tuple<TransferHubModel, string> GetValue(string key)
        {
            Tuple<TransferHubModel, string> value;
            if (_dicStore.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        public void Remove(string key)
        {
            lock (_dicStore)
            {
                if (_dicStore.ContainsKey(key))
                {
                    _dicStore.Remove(key);
                }
            }
        }
    }
}