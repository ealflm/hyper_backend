using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement
{
    public class VehicleViewModel
    {
        public Guid Id { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public Guid? RentStationId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? PriceRentingId { get; set; }
        public string Name { get; set; }
        public string VehicleTypeName { get; set; }
        public string LicensePlates { get; set; }
        public string ServiceTypeName { get; set; }
        public string CompanyName { get; set; }
        public string Color { get; set; }
        public int Status { get; set; }
    }
}