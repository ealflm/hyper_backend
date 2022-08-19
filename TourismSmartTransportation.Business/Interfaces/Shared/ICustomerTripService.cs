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
        Task<List<CustomerTripViewModel>> GetCustomerTrips();
        Task<List<CustomerTripViewModel>> GetCustomerTrips(Guid partnerId);
        Task<List<CustomerTripViewModel>> GetCustomerTripsForDriver(Guid driverId);
        Task<Response> UpdateStatusCustomerTrip(Guid id, CustomerTripSearchModel model);

        Task<Response> ReturnVehicle(Guid customerTripId);
    }
}