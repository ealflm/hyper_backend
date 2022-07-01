using System;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail
{
    public class OrderDetailsInfo
    {
        public Guid? PackageId { get; set; }
        public Guid? PriceOfBusServiceId { get; set; }
        public Guid? PriceOfBookingServiceId { get; set; }
        public Guid? PriceOfRentingServiceId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Content { get; set; }
    }
}
