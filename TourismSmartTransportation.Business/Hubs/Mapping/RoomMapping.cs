using System.Collections.Generic;

namespace TourismSmartTransportation.Business.Hubs.Mapping
{
    public class RoomMapping
    {
        private readonly Dictionary<string, string> _rooms =
            new Dictionary<string, string>();

        public int Count
        {
            get
            {
                return _rooms.Count;
            }
        }

        public Dictionary<string, string> GetRooms()
        {
            return _rooms;
        }

        public string GetValue(string key)
        {
            string value;
            lock (_rooms)
            {
                if (_rooms.TryGetValue(key, out value))
                {
                    return value;
                }
            }

            return null;
        }

        public void Add(string key, string value)
        {
            lock (_rooms)
            {
                if (!_rooms.ContainsKey(key))
                {
                    _rooms.Add(key, value);
                }

                _rooms[key] = value;
            }
        }

        public void Remove(string key)
        {
            lock (_rooms)
            {
                if (_rooms.ContainsKey(key))
                {
                    _rooms.Remove(key);
                }
            }
        }
    }
}