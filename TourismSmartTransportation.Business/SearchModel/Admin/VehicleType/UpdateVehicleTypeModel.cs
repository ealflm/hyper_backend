using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.VehicleType
{
    public class UpdateVehicleTypeModel
    {
        [NotAllowedEmptyStringValidator]
        public string Label { get; set; }

        [Range(1, 70)]
        public int? Seats { get; set; }

        [NotAllowedEmptyStringValidator]
        public string Fuel { get; set; }

        public int? Status { get; set; }
    }
}