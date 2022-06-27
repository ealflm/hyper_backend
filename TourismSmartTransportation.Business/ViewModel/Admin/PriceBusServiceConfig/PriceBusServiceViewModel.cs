using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PriceBusServiceViewModel
{
    public class PriceBusServiceViewModel
    {
        public Guid Id { get; set; }
        public decimal MinRouteDistance { get; set; }
        public decimal MaxRouteDistance { get; set; }
        public decimal MinDistance { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal Price { get; set; }
        public decimal MinStation { get; set; }
        public decimal MaxStation { get; set; }
        public string Mode { get; set; }
        public int Status { get; set; }
    }
}