using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.StationManagement
{
    public class StationSearchModel: PagingSearchModel
    {
        [NotAllowedEmptyStringValidator]
        public string Name { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Address { get; set; }
        [Range(1,2)]
        public int? Status { get; set; }
    }
}
