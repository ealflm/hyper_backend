using System;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Tier
{
    public class TierSearchModel : PagingSearchModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromotedTitle { get; set; }
        public int? Status { get; set; }
    }
}