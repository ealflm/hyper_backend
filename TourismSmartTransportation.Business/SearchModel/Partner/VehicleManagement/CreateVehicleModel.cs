using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class CreateVehicleModel
    {
        [Required]
        public Guid ServiceTypeId { get; set; }
        [Required]
        public Guid VehicleTypeId { get; set; }
        public Guid? RentStationId { get; set; }
        [Required]
        public Guid PartnerId { get; set; }
        public Guid? PriceRentingId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LicensePlates { get; set; }
        [Required]
        public string Color { get; set; }
    }
}