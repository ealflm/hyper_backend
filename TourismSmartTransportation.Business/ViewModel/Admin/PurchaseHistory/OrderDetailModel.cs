using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory
{
    public class OrderDetailModel
    {
        public virtual Guid OrderId { get; set; }
        public virtual decimal Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string Content { get; set; }
        public virtual int Status { get; set; }
    }
}
