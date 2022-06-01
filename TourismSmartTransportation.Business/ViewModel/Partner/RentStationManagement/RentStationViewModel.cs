using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement
{
    public class RentStationViewModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public Guid PartnerId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Status { get; set; }
    }
}
