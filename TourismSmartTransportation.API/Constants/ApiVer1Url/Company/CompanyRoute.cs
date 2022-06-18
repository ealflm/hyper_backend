﻿using TourismSmartTransportation.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Partner
    {
        // Base url
        public const string Role = "partner";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";
        public const string Route = BaseApiUrl + "/routes";

        // Vehicle
        public const string Vehicle = BaseApiUrl + "/vehicle";

        // Driver
        public const string Driver = BaseApiUrl + "/driver";

        // RentStation
        public const string RentStation = BaseApiUrl + "/rent-station";

        // Configuration
        public const string GetConfiguration = BaseApiUrl + "/get-config";

        // Service type
        public const string ServiceType = BaseApiUrl + "/service-type";
    }
}
