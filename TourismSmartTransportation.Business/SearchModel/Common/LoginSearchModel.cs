using System.ComponentModel.DataAnnotations;


namespace TourismSmartTransportation.Business.SearchModel.Common.Authorization
{
    public class LoginSearchModel
    {
        [Required] public string UserName { get; set; }

        [Required] public string Password { get; set; }
    }
}
