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

        //order
        public const string Order = BaseApiUrl + "/order";

        // rent station
        public const string RentStation = BaseApiUrl + "/rentStation";

        // bus trip
        public const string BusTrip = BaseApiUrl + "/bus-trip";

        //purchase history
        public const string PurchaseHistory = BaseApiUrl + "/purchase-history";

        // save registration token (firebase service)
        public const string Firebase = BaseApiUrl + "/firebase";

        //rent customer trip
        public const string RentCustomerTrip = BaseApiUrl + "/rent-service-customer-trip";

        //pay bus trip mobile
        public const string PayBusTripMobile = BaseApiUrl + "/bus-trip-pay-mobile";

        //pay bus trip mobile
        public const string PayBusTripIOT = BaseApiUrl + "/bus-trip-pay-iot";

        // station
        public const string Station = BaseApiUrl + "/station";

        //return vehicle
        public const string ReturnVehicle = BaseApiUrl + "/return-vehicle";

        // notification
        public const string Notification = BaseApiUrl + "/notification";
    }
}
