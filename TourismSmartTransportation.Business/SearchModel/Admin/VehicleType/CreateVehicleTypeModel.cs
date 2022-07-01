using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.VehicleType
{
    public class CreateVehicleTypeModel
    {
        [Required]
        [StringLength(255)]
        public string Label { get; set; }

        [Required]
        [Range(1, 70)]
        public int Seats { get; set; }

        [Required]
        public string Fuel { get; set; }
    }
}