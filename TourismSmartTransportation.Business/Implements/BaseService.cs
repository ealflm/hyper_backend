using System;
using System.Globalization;
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

        public static int GetPageSize(int itemPerPage, int totalRecord)
        {
            return itemPerPage == 0 ? 1 : (totalRecord / itemPerPage) + (totalRecord % itemPerPage > 0 ? 1 : 0);
        }

        public static string SortBy(string sortByField, string defaultField)
        {
            if (sortByField == null || sortByField.Trim() == "")
                sortByField = defaultField;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sortByField.ToLower()); ;
        }
    }
}