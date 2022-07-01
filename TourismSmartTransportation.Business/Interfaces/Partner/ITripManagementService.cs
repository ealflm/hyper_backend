using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface ITripManagementService
    {
        Task<SearchResultViewModel<TripViewModel>> GetTripsList(TripSearchModel model);
        Task<Response> CreateTrip(CreateTripModel model);
        Task<Response> UpdateTrip(Guid id, UpdateTripModel model);
        Task<Response> DeleteTrip(Guid id);
        Task<TripViewModel> GetTripById(Guid id);
    }
}
