using System.Collections.Generic;

namespace TourismSmartTransportation.Business.ViewModel.Common
{
    public class SearchResultViewModel<T> where T : class
    {
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }

        public SearchResultViewModel() { }

        public SearchResultViewModel(List<T> items, int pageSize, int totalItems)
        {
            PageSize = pageSize;
            TotalItems = totalItems;
            Items = items;
        }
    }
}