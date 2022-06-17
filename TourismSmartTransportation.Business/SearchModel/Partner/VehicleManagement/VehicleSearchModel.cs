using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class VehicleSearchModel
    {
        public Guid? ServiceTypeId { get; set; }
        public Guid? VehicleTypeId { get; set; }
        public Guid? RentStationId { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? PriceRentingId { get; set; }
        public string Name { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public int? IsRunning { get; set; }
        public int? Status { get; set; }
    }
}