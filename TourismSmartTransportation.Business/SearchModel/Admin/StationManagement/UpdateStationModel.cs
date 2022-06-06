using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.StationManagement
{
    public class UpdateStationModel
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? Status { get; set; }
    }
}
