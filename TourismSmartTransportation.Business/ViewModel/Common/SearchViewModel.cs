using System.Collections.Generic;

namespace TourismSmartTransportation.Business.ViewModel.Common
{
    public class SearchResultViewModel<T> where T : class
    {
        public List<T> Items { get; set; }
        public int PageSize { get; set; }
    }
}