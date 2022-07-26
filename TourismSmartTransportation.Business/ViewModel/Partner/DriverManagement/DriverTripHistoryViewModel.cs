using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement
{
    public class DriverTripHistoryViewModel
    {
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlates { get; set; }
        public Guid DriverId { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public Guid RouteId { get; set; }
        public string RouteName { get; set; }
        public decimal? Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}