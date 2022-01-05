using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validator;

namespace TourismSmartTransportation.Business.SearchModel.Vehicle
{
    public class VehicleTypeSearchModel
    {
        [NotAllowedEmptyString]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, 70)]
        public int? Seats { get; set; }

        [Range(0, 2)]
        public int? Fuel { get; set; }

        [Range(0, 1)]
        public int? Status { get; set; }
    }
}