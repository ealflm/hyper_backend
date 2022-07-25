using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig
{
    public class PriceBusServiceSearchModel
    {
        public Guid? BasePriceBusServiceId { get; set; }

        [ValidValueMinMax("MaxRouteDistance", ErrorMessage = "MinRouteDistance cannot be greater than MaxRouteDistance")]
        public decimal? MinRouteDistance { get; set; }

        public decimal? MaxRouteDistance { get; set; }

        [ValidValueMinMax("MaxDistance", ErrorMessage = "MinDistance cannot be greater than MaxDistance")]
        public decimal? MinDistance { get; set; }

        public decimal? MaxDistance { get; set; }

        public decimal? Price { get; set; }

        [ValidValueMinMax("MinStation", ErrorMessage = "MinStation cannot be greater than MaxStation")]
        public decimal? MinStation { get; set; }

        public decimal? MaxStation { get; set; }

        public string Mode { get; set; }

        public int? Status { get; set; }
    }
}