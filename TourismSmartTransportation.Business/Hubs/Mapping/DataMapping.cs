using TourismSmartTransportation.Business.Hubs.Models;

namespace TourismSmartTransportation.Business.Hubs.Mapping
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

        public Mapping<DataHubModel> GetDrivers(DriverStatus status = DriverStatus.Active)
        {
            if (status == DriverStatus.On)
            {
                Mapping<DataHubModel> driversWithStatus = new Mapping<DataHubModel>();
                foreach (var item in _driversData.GetItems())
                {
                    if (item.Value.Status == (int)status)
                    {
                        driversWithStatus.Add(item.Key, item.Value);
                    }
                }
                return driversWithStatus;
            }
            return _driversData;
        }

        public void Add(string key, DataHubModel value, User type)
        {
            if (type == User.Customer)
            {
                _customersData.Add(key, value);
                return;
            }

            _driversData.Add(key, value);
        }

        public DataHubModel GetValue(string key, User type)
        {
            if (type == User.Customer)
            {
                return _customersData.GetValue(key);
            }

            return _driversData.GetValue(key);
        }

        public void Remove(string key, User type)
        {
            if (type == User.Customer)
            {
                _customersData.Remove(key);
                return;
            }

            _driversData.Remove(key);
        }
    }
}