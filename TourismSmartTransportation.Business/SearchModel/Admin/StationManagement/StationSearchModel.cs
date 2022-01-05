using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.StationManagement
{
    public class StationSearchModel: PagingSearchModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
    }
}
