using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Common
{
    public class SearchResultViewModel
    {
        public List<object> Items { get; set; }
        public int PageSize { get; set; }
    }
}