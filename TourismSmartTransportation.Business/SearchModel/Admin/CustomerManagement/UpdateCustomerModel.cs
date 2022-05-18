using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.CustomerManagement
{
    public class UpdateCustomerModel : FileViewModel
    {
        public Guid? TierId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhotoUrl { get; set; }
        [AllowNullOrEmptyAndCheckValidPhone]
        public string Phone { get; set; }
        [AllowEmptyAndChekcValidEmail]
        [StringLength(50)]
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Status { get; set; }
    }
}
