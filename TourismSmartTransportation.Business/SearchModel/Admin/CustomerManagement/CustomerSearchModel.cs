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
        [NotAllowedEmptyStringValidator]
        public string FirstName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string LastName { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Range(1,2)]
        public int? Status { get; set; }
    }
}
