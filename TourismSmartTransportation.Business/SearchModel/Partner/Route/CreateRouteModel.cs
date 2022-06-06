using System;
using System.Collections.Generic;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Route
{
    public class CreateRouteModel
    {
        public Guid PartnerId { get; set; }
        public string Name { get; set; }
        public int TotalStation { get; set; }
        public decimal Distance { get; set; }
        public List<CreateStationRoute> StationList { get; set; }
    }
}