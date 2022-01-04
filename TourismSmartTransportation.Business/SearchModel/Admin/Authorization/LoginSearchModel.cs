using System.ComponentModel.DataAnnotations;


namespace TourismSmartTransportation.Business.SearchModel.Admin.Authorization
{
    public class LoginSearchModel
    {
        [Required, EmailAddress] public string Email { get; set; }

        [Required] public string Password { get; set; }
    }
}
