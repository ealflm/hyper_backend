using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig
{
    public class CreatePriceBusServiceModel
    {
        [Required]
        [ValidValueMinMax("MaxRouteDistance", ErrorMessage = "MinRouteDistance cannot be greater than MaxRouteDistance")]
        public decimal MinRouteDistance { get; set; }
        [Required]
        public decimal MaxRouteDistance { get; set; }
        [Required]
        [ValidValueMinMax("MaxDistance", ErrorMessage = "MinDistance cannot be greater than MaxDistance")]
        public decimal MinDistance { get; set; }
        [Required]
        public decimal MaxDistance { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [ValidValueMinMax("MinStation", ErrorMessage = "MinStation cannot be greater than MaxStation")]
        public decimal MinStation { get; set; }
        [Required]
        public decimal MaxStation { get; set; }
        [Required]
        public string Mode { get; set; }
    }
}