using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface ICustomerTripService
    {
        Task<List<CustomerTripViewModel>> GetCustomerTripsListForRentingService(CustomerTripSearchModel model);
        Task<Response> UpdateStatusCustomerTrip(Guid id, CustomerTripSearchModel model);
    }
}