using System;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class SendDiscountToCustomer
    {
        public Guid DiscountId { get; set; }
        public Guid CustomerId { get; set; }
    }
}