using System;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order
{
    public class MakeOrderTestModel
    {
        public Guid CustomerId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? DiscountId { get; set; }
        public decimal TotalPrice { get; set; }
        public Guid? PackageId { get; set; }
        public Guid? PriceOfBusServiceId { get; set; }
        public Guid? PriceOfBookingServiceId { get; set; }
        public Guid? PriceOfRentingServiceId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Content { get; set; }
    }
}
