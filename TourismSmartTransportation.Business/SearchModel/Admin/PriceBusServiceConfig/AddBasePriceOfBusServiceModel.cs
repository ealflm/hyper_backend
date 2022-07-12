using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig
{
    public class AddBasePriceOfBusService
    {
        [Required]
        public decimal MaxDistance { get; set; }

        [Required]
        public decimal MinDistance { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}