using System;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class PackageDetailsViewModel
    {
        public Guid ServiceTypeId { get; set; }
        public string ServiceName { get; set; }
        public decimal LimitValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal DiscountValue { get; set; }
    }
}