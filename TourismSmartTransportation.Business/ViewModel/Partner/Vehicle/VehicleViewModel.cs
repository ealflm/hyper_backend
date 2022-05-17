using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.Vehicle
{
    public class VehicleViewModel
    {
        public Guid Id { get; set; }
        public string ServiceTypeId { get; set; }
        public string VehicleTypeId { get; set; }
        public string RentStationId { get; set; }
        public string PartnerId { get; set; }
        public string CategoryId { get; set; }
        public string PublishYearId { get; set; }
        public string ActivityDateId { get; set; }
        public string Name { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Status { get; set; }
    }
}