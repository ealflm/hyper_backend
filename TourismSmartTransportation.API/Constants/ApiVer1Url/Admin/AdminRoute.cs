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

        // Payment
        public const string Transaction = BaseApiUrl + "/transaction";


        // Service type
        public const string ServiceType = BaseApiUrl + "/service-type";

        // Customer Package History
        public const string CustomerPackageHistory = BaseApiUrl + "/package-history";

        // Package
        public const string Package = BaseApiUrl + "/package";

        //Route
        public const string Route = BaseApiUrl + "/route";

        // Price Bus Service Config
        public const string PriceBusConfig = BaseApiUrl + "/price-bus-service-config";

        // Category
        public const string Category = BaseApiUrl + "/category";

        // Publish year
        public const string PublishYear = BaseApiUrl + "/publish-year";

        // Price Booking Service Config
        public const string PriceBookingConfig = BaseApiUrl + "/price-booking-service-config";

        // Price Renting Service Config
        public const string PriceRentingConfig = BaseApiUrl + "/price-renting-service-config";

        // Card
        public const string Card = BaseApiUrl + "/card";

        // Tracking Vehicle
        public const string TrackingVehicle = BaseApiUrl + "/tracking-vehicle";

        // Vehicle
        public const string Vehicle = BaseApiUrl + "/vehicle";

        // Driver
        public const string Driver = BaseApiUrl + "/driver";

        //Power BI
        public const string PowerBI = BaseApiUrl + "/power-bi";

        // Base price of bus service
        public const string BasePriceOfBusService = BaseApiUrl + "/base-price-bus-service";

        // wallet
        public const string Wallet = BaseApiUrl + "/wallet";

        // customer trip
        public const string CustomerTrip = BaseApiUrl + "/customer-trip";
    }
}