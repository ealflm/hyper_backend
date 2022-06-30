using System;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class CustomerPackagesHistoryViewModel
    {
        public Guid CustomerId { get; set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }
    }
}