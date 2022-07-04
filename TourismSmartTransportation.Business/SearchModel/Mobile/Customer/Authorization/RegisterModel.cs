
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer.Authorization
{
    public class RegisterModel : FileViewModel
    {
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Pin { get; set; }
        [NotAllowedEmptyStringValidator]
        public string FirstName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string LastName { get; set; }
        public bool Gender { get; set; }
    }
}
