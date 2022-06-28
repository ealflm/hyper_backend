using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class CreateVehicleModel : VehicleModel
    {
        [Required]
        public override Guid? ServiceTypeId { get; set; }

        [Required]
        public override Guid? VehicleTypeId { get; set; }

        [Required]
        public override Guid? PartnerId { get; set; }

        [Required]
        public override string Name { get; set; }

        [Required]
        public override string LicensePlates { get; set; }

        [Required]
        public override string Color { get; set; }
    }
}