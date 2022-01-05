using TourismSmartTransportation.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Admin
    {
        // Base url
        public const string Role = "admin";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";
        public const string Register = BaseApiUrl + "/authorization/register";

        // Vehicle
        public const string VehicleType = BaseApiUrl + "/vehicles/types";
    }
}