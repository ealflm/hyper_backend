using System;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order
{
    public class CreateOrderModel
    {
        public Guid CustomerId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? DiscountId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
