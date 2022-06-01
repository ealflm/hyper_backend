using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement
{
    public class RentStationSearchModel : PagingSearchModel
    {
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? Status { get; set; }
    }
}
