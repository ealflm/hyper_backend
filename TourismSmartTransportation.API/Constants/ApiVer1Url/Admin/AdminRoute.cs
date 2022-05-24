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

        // Station
        public const string Station = BaseApiUrl + "/stations";

        // Discount
        public const string Discount = BaseApiUrl + "/discounts";

        // Company
        public const string Partner = BaseApiUrl + "/partners";

        // Serivice
        public const string Service = BaseApiUrl + "/service-management";

        // Customer
        public const string Customer = BaseApiUrl + "/customers";

        // RentStation
        public const string RentStation = BaseApiUrl + "/rent-station";

        // Order
        public const string Order = BaseApiUrl + "/order";

        // Order detail
        public const string OrderDetail = BaseApiUrl + "/order-detail";

        // Transaction
        public const string Transaction = BaseApiUrl + "/transaction";


        // Service type
        public const string ServiceType = BaseApiUrl + "/service-type";

        // Customer Tier History
        public const string CustomerTierHistory = BaseApiUrl + "/cus-tier-his";

        // Tier
        public const string Tier = BaseApiUrl + "/tier";

    }
}