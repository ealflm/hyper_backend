// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace TourismSmartTransportation.Business.ViewModel.Common
// {
//     public class PagedList<T> : List<T>
//     {
//         public List<T> Items { get; set; }
//         public int PageSize { get; set; }
//         public int TotalItems { get; set; }

//         public PagedList() { }

//         public PagedList(List<T> items, int itemsPerPage, int pageIndex, int totalItems)
//         {
//             PageSize = itemsPerPage == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)itemsPerPage);
//             TotalItems = totalItems;
//             Items = pageIndex > PageSize ? null : items;

//             AddRange(items);
//         }

//         public static PagedList<T> ToPagedList(IQueryable<T> source, int pageIndex, int itemsPerPage)
//         {
//             var totalItems = source.Count();
//             var items = source.Skip(itemsPerPage < totalItems ? itemsPerPage * Math.Max(pageIndex - 1, 0) : 0)
//                     .Take(itemsPerPage < totalItems && itemsPerPage > 0 ? itemsPerPage : totalItems)
//                     .ToList();

//             return new PagedList<T>(items, itemsPerPage, pageIndex, totalItems);
//         }
//     }
// }