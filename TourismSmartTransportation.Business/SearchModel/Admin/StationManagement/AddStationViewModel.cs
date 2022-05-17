using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.StationManagement
{
    public class AddStationViewModel
    {
        [NotAllowedEmptyStringValidator]
        public string Title { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? Status { get; set; }
    }
}
