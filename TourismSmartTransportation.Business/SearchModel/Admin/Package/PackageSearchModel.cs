using System;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Package
{
    public class PackageSearchModel : PagingSearchModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromotedTitle { get; set; }
        public int? Status { get; set; }
    }
}