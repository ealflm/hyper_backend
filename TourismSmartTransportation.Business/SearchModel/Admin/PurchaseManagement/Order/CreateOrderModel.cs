using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order
{
    public class CreateOrderModel
    {
        public Guid CustomerId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? DiscountId { get; set; }
        public Guid? PartnerId { get; set; }
        public List<OrderDetailsInfo> OrderDetailsInfos { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
