using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement
{
    public class TripViewModel
    {
        public Guid TripId { get; set; }
        public Guid DriverId { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverPhotoUrl { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlates { get; set; }
        public Guid RouteId { get; set; }
        public string TripName { get; set; }
        public int DayOfWeek { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int Status { get; set; }
    }
}
