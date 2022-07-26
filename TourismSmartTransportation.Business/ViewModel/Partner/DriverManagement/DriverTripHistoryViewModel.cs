using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement
{
    public class DriverTripHistoryViewModel : DriverViewModel
    {
        public Guid CustomerTripId { get; set; }
        // public Guid VehicleId { get; set; }
        // public string VehicleName { get; set; }
        // public string LicensePlates { get; set; }
        public Guid ServiceTypeId { get; set; }
        // public string ServiceTypeName { get; set; }
        // public Guid DriverId { get; set; }
        // public string DriverFirstName { get; set; }
        // public string DriverLastName { get; set; }
        public Guid? RouteId { get; set; }
        public string RouteName { get; set; }
        // public int FeedbackDriverRating { get; set; }
        public decimal? Distance { get; set; }
        public DateTime CreatedDateDriverHis { get; set; }
        public DateTime ModifiedDateDriverHis { get; set; }
    }
}