using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.SearchModel.Partner.TripManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface ITripManagementService
    {
        Task<SearchResultViewModel<TripViewModel>> GetTripsList(TripSearchModel model);
        Task<Response> CreateTrip(CreateTripModel model);
        Task<Response> UpdateTrip(Guid id, UpdateTripModel model);
        Task<Response> DeleteTrip(Guid id);
        Task<Response> CopyTrip(CopyTripModel model);
        Task<TripViewModel> GetTripById(Guid id);
        Task<List<VehicleViewModel>> GetVehicleListDropdownOptions(VehicleDropdownOptionsTripModel model);
        Task<List<DriverViewModel>> GetDriverListDropdownOptions(DriverDropdownOptionsTripModel model);
    }
}
