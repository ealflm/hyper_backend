using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.CustomerManagement
{
    public class CustomerSearchModel : PagingSearchModel
    {
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [AllowNullOrEmptyAndCheckValidPhone]
        public string Phone { get; set; }
        public int? Status { get; set; }
    }
}
