namespace TourismSmartTransportation.Business.Hubs
{
    public class RoomMapping
    {
        private readonly Mapping<string> _customers = new Mapping<string>();
        private readonly Mapping<string> _drivers = new Mapping<string>();

        public int CustomerCount
        {
            get
            {
                return _customers.Count;
            }
        }

        public int DriverCount
        {
            get
            {
                return _drivers.Count;
            }
        }

        public void Add(string key, string value, bool isCustomer = true)
        {
            if (isCustomer)
            {
                _customers.Add(key, value);
            }

            _drivers.Add(value, key);
        }

        public string GetValue(string key, bool isCustomer = true)
        {
            if (isCustomer)
            {
                return _customers.GetValue(key);
            }

            return _drivers.GetValue(key);
        }

        public void Remove(string key, bool isCustomer = true)
        {
            if (isCustomer)
            {
                _customers.Remove(key);
            }

            _drivers.Remove(_customers.GetValue(key));
        }
    }
}