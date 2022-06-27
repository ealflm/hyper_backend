using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory
{
    public class OrderDetailOfBookingServiceViewModel
    {
        public Guid OrderId { get; set; }
        public Guid PriceOfBookingServiceId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    }
}
