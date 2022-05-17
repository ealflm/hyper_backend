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
    public class AddCustomerViewModel : FileViewModel
    {
        public string TierId { get; set; }
        [NotAllowedEmptyStringValidator]
        [StringLength(50)]
        public string FirstName { get; set; }
        [NotAllowedEmptyStringValidator]
        [StringLength(50)]
        public string LastName { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhotoUrl { get; set; }
        [Phone]
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Password { get; set; }
        [Range(1, 2)]
        public int? Status { get; set; }
    }
}
