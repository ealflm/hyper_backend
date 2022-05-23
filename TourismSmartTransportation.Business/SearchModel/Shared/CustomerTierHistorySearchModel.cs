using System;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Shared
{
    public class CustomerTierHistorySearchModel : PagingSearchModel
    {
        public Guid? CustomerId { get; set; }
        public Guid? TierId { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? Status { get; set; }
    }
}