using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Route
{
    public class TripSearchModel : PagingSearchModel
    {
        public string TripName { get; set; }
        public Guid? PartnerId { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
    }
}