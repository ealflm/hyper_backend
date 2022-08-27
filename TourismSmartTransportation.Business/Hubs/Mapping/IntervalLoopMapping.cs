using System;
using System.Collections.Generic;

namespace TourismSmartTransportation.Business.Hubs.Mapping
{
    public class IntervalLoopMapping
    {
        // CustomerId | CustomerTime | DriverId | DriverTime
        private readonly Dictionary<string, Tuple<DateTime, string, DateTime>> _maps =
            new Dictionary<string, Tuple<DateTime, string, DateTime>>();

        public int Count
        {
            get
            {
                return _maps.Count;
            }
        }

        public Dictionary<string, Tuple<DateTime, string, DateTime>> GetItems()
        {
            return _maps;
        }

        public Tuple<DateTime, string, DateTime> GetValue(string key)
        {
            Tuple<DateTime, string, DateTime> value;
            lock (_maps)
            {
                if (_maps.ContainsKey(key))
                {
                    if (_maps.TryGetValue(key, out value))
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        public void Add(string key, Tuple<DateTime, string, DateTime> value)
        {
            lock (_maps)
            {
                if (!_maps.ContainsKey(key))
                {
                    _maps.Add(key, value);
                }

                _maps[key] = value;
            }
        }

        public void Remove(string key)
        {
            lock (_maps)
            {
                if (_maps.ContainsKey(key))
                {
                    _maps.Remove(key);
                }
            }
        }
    }
}