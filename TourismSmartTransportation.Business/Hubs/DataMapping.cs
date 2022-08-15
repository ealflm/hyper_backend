namespace TourismSmartTransportation.Business.Hubs
{
    public class DataMapping
    {
        private readonly Mapping<DataHubModel> _customersData = new Mapping<DataHubModel>();
        private readonly Mapping<DataHubModel> _driversData = new Mapping<DataHubModel>();

        public int CustomerDataCount
        {
            get
            {
                return _customersData.Count;
            }
        }

        public int DriverDataCount
        {
            get
            {
                return _driversData.Count;
            }
        }

        public Mapping<DataHubModel> GetCustomers()
        {
            return _customersData;
        }

        public Mapping<DataHubModel> GetDrivers()
        {
            return _driversData;
        }

        public void Add(string key, DataHubModel value, bool isCustomer = true)
        {
            if (isCustomer)
            {
                _customersData.Add(key, value);
                return;
            }

            _driversData.Add(key, value);
        }

        public DataHubModel GetValue(string key, bool isCustomer = true)
        {
            if (isCustomer)
            {
                return _customersData.GetValue(key);
            }

            return _driversData.GetValue(key);
        }

        public void Remove(string key, bool isCustomer = true)
        {
            if (isCustomer)
            {
                _customersData.Remove(key);
                return;
            }

            _driversData.Remove(key);
        }
    }
}