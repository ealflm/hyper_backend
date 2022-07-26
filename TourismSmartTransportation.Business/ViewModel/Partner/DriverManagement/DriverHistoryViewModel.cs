using System;
using System.Collections.Generic;

namespace TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement
{
    public class DriverHistoryViewModel
    {
        public DriverViewModel Driver { get; set; }
        public List<History> History { get; set; }
    }

    public class History
    {
        public Guid CustomerTripId { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlates { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public Guid? RouteId { get; set; }
        public string RouteName { get; set; }
        public int FeedbackDriverRating { get; set; }
        public decimal? Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}