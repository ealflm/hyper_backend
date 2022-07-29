using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class BusPaySearchModel
    {
        
        public Guid VehicleId { get; set; }
        public Guid CustomerId { get; set; }
        [Required]
        public string Uid { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public decimal Latitude { get; set; }
    }
}
