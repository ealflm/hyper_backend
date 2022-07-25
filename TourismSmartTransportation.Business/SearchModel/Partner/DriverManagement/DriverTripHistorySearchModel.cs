using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement
{
    public class DriverTripHistorySearchModel : PagingSearchModel
    {
        [Required]
        public Guid DriverId { get; set; }
        public Guid? VehicleId { get; set; }
        public int? Status { get; set; }
    }
}