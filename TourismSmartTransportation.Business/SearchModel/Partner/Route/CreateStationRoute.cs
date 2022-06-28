using System;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Route
{
    public class CreateStationRoute
    {
        public int OrderNumber { get; set; }
        public Guid StationId { get; set; }
        public Guid RouteId { get; set; }
        public decimal Distance { get; set; }
    }
}