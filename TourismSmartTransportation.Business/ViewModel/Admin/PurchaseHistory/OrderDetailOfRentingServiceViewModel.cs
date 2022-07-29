using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory
{
    public class OrderDetailOfRentingServiceViewModel : OrderDetailModel
    {
        public Guid OrderDetailId { get; set; }
        public Guid? PriceOfRentingServiceId { get; set; }
    }
}
