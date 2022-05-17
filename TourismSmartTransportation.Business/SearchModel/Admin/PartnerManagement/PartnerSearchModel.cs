using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement
{
    public class PartnerSearchModel : PagingSearchModel
    {
        [NotAllowedEmptyStringValidator]
        public string Username { get; set; }
        [NotAllowedEmptyStringValidator]
        public int? Status { get; set; }
    }
}