using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements
{
    public class BaseService : IBaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public static T UpdateTypeOfNullAbleObject<T>(T oldValue, T newValue)
        {
            return newValue != null ? newValue : oldValue;
        }
        public static T UpdateTypeOfNotNullAbleObject<T>(T oldValue, T? newValue) where T : struct
        {
            return newValue != null ? newValue.Value : oldValue;
        }

        public static int SkipItemsOfPagingFunc(int itemPerPage, int totalRecord, int pageIndex)
        {
            return itemPerPage < totalRecord ? itemPerPage * Math.Max(pageIndex - 1, 0) : 0;
        }

        public static int TakeItemsOfPagingFunc(int itemPerPage, int totalRecord)
        {
            return itemPerPage < totalRecord && itemPerPage > 0 ? itemPerPage : totalRecord;
        }

        // Get number of pages need to show on UI
        public static int GetPageSize(int itemPerPage, int totalRecord)
        {
            return itemPerPage == 0 ? 1 : (totalRecord / itemPerPage) + (totalRecord % itemPerPage > 0 ? 1 : 0);
        }


        // Function using for method OrderBySingleField
        public static string SortBy(string sortByField, string defaultField)
        {
            if (sortByField == null || sortByField.Trim() == "")
                sortByField = defaultField;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sortByField.ToLower()); ;
        }


        // Return list after sorting
        public static List<T> GetListAfterSorting<T>(List<T> list, string sortBy) where T : class
        {
            var result = string.IsNullOrWhiteSpace(sortBy)
                            ? list.AsQueryable().ToList()
                            : list.AsQueryable().OrderByMultipleFields(sortBy).ToList();
            return result;
        }

        // Return total records from list
        public static int GetTotalRecord<T>(List<T> list,
                                                int itemsPerPage, int pageIndex) where T : class
        {
            var totalRecord = list.Count();
            if (GetPageSize(itemsPerPage, totalRecord) < pageIndex)
                totalRecord = 0;

            return totalRecord;
        }

        // Return list after paging
        public static List<T> GetListAfterPaging<T>(List<T> list, int itemsPerPage,
                                    int pageIndex, int totalRecord) where T : class
        {
            var result = list.AsQueryable()
                                        .Skip(SkipItemsOfPagingFunc(itemsPerPage, totalRecord, pageIndex))
                                        .Take(TakeItemsOfPagingFunc(itemsPerPage, totalRecord))
                                        .ToList();

            return result;
        }
    }
}