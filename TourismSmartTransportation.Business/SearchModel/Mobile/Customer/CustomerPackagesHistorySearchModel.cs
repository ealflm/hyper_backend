using System;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class CustomerPackagesHistorySearchModel : PagingSearchModel
    {
        public Guid? CustomerId { get; set; }
        public Guid? PackageId { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? Status { get; set; }
    }
}