using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Validate;

namespace TourismSmartTransportation.Business.ViewModel.Admin.StationManagement
{
    public class AddStationViewModel
    {
        [NotAllowedEmptyStringValidator]
        public string Name { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        [Range(1,2)]
        public int? Status { get; set; }
    }
}
