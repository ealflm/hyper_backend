using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.StationManagement
{
    public class SearchStationResultViewModel
    {
        public List<StationViewModel> Items { get; set; }
        public int PageSize { get; set; }
    }
}
