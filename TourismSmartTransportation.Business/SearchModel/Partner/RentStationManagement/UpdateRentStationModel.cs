using System;

namespace TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement
{
    public class UpdateRentStation
    {
        public Guid? PartnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? Status { get; set; }
    }
}
