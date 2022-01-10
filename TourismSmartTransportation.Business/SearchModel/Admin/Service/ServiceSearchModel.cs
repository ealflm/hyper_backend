using System;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Service
{
    public class ServiceSearchModel : PagingSearchModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Time { get; set; }
        public int? Status { get; set; }
    }
}