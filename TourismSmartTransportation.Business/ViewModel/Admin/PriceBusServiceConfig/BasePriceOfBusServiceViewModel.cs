using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PriceBusServiceViewModel
{
    public class BasePriceOfBusServiceViewModel
    {
        public Guid Id { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal MinDistance { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
    }
}