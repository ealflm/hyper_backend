
using System.ComponentModel.DataAnnotations;


namespace TourismSmartTransportation.Business.SearchModel.Admin.Authorization
{
    public class RegisterSearchModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
