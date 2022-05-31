using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;

namespace TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement
{
    public class RouteViewModel
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Name { get; set; }
        public int TotalStation { get; set; }
        public decimal Distance { get; set; }
        public List<StationViewModel> StationList { get; set; }
        public int Status { get; set; }
    }
}
