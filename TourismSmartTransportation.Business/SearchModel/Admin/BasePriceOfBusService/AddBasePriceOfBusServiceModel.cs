using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.BasePriceOfBusService
{
    public class AddBasePriceOfBusService
    {
        [Required]
        public decimal MaxDistance { get; set; }

        [Required]
        [ValidValueMinMax("MaxDistance", ErrorMessage = "MinDistance cannot be greater than MaxDistance")]
        public decimal MinDistance { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}