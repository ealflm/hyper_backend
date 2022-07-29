using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
     public class CustomerTripSearchModel
    {
        [Required]
        public Guid CustomerId { get; set; }
        public Guid? RouteId { get; set; }
        [Required]
        public Guid VehicleId { get; set; }
        public decimal? Distance { get; set; }
        public DateTime? RentDeadline { get; set; }
        public string Coordinates { get; set; }
    }
}
