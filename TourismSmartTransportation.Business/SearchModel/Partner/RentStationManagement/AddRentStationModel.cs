using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement
{
    public class AddRentStationModel
    {
        public Guid PartnerId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
