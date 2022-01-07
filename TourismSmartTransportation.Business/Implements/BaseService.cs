using System;
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

        public static T UpdateStringFilter<T>(T oldValue, T newValue)
        {
            return (newValue != null && !newValue.Equals(oldValue)) ? newValue : oldValue;
        }
        public static T UpdateNumberFilter<T>(T oldValue, T? newValue) where T : struct
        {
            return (newValue != null && !newValue.Value.Equals(oldValue)) ? newValue.Value : oldValue;
        }
    }
}