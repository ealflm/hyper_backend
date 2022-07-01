using TourismSmartTransportation.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Customer
    {
        // Base url
        public const string Role = "customer";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";

        // Package history
        public const string CustomerPackageHistory = BaseApiUrl + "/package-history";
    }
}
