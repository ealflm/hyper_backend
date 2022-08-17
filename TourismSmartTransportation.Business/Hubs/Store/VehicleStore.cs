using System.Collections.Generic;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;

namespace TourismSmartTransportation.Business.Hubs.Store
{
    public class VehicleStore
    {
        public Dictionary<string, VehicleViewModel> VehiclesList = new Dictionary<string, VehicleViewModel>();
    }
}