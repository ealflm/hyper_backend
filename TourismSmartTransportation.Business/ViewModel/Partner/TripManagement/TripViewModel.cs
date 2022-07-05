using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement
{
    public class TripViewModel
    {
        public Guid TripId { get; set; }
        public Guid DriverId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid VehicleId { get; set; }
        public string LicensePlates { get; set; }
        public Guid RouteId { get; set; }
        public string TripName { get; set; }
        public int DayOfWeek { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }
    }
}