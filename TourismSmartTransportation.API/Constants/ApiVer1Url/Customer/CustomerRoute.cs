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
        public const string Register = BaseApiUrl + "/authorization/register";
        public const string CheckPhoneNumber = BaseApiUrl + "/authorization/login/verify";

        // Package history
        public const string CustomerPackageHistory = BaseApiUrl + "/package-history";

        // OTP Verification
        public const string OTP = BaseApiUrl + "/otp";

        // Deposit
        public const string Deposit = BaseApiUrl + "/deposit";

        // Deposit MoMo
        public const string DepositMoMo = BaseApiUrl + "/deposit-momo";

        // Wallet
        public const string Wallet = BaseApiUrl + "/wallet";

        // package list
        public const string Package = BaseApiUrl + "/package";

        // Card matching
        public const string CardMatch = BaseApiUrl + "/card-match";

        // package-details
        public const string PackageDetails = BaseApiUrl + "/package-details";

        //rent service
        public const string RentService = BaseApiUrl + "/rent-service";
    }
}
