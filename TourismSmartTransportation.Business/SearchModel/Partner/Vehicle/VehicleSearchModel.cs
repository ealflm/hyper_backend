using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Vehicle
{
    public class VehicleSearchModel
    {
        [NotAllowedEmptyStringValidator]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, 70)]
        public int? Seats { get; set; }

        [Range(0, 2)]
        public int? Fuel { get; set; }

        [Range(1, 2)]
        public int? Status { get; set; }
    }
}