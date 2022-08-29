using TourismSmartTransportation.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Driver
    {
        // Base url
        public const string Role = "driver";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";

        // OTP
        public const string OTP = BaseApiUrl + "/otp";

        // check number
        public const string CheckPhoneNumber = BaseApiUrl + "/authorization/login/verify";

        // customer trip
        public const string CustomerTrip = BaseApiUrl + "/customer-trip";

        // change password
        public const string ChangePassword = BaseApiUrl + "/authorization/change-password";

        // save registration token (firebase service)
        public const string Firebase = BaseApiUrl + "/firebase";

        // Notification
        public const string Notification = BaseApiUrl + "/notification";
    }
}
