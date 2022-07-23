using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class VehicleDropdownOptionsTripModel
    {
        [Required]
        public Guid PartnerId { get; set; }

        [Required]
        public Guid ServiceTypeId { get; set; }
    }
}