using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.VehicleType
{
    public class CreateVehicleTypeModel
    {
        [NotAllowedEmptyStringValidator]
        [StringLength(255)]
        public string Label { get; set; }

        [Range(1, 70)]
        public int Seats { get; set; }

        public string Fuel { get; set; }

        public decimal Price { get; set; }

        public int? Status { get; set; }
    }
}