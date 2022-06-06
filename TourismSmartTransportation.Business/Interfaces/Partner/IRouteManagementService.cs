using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface IRouteManagementService
    {
        Task<SearchResultViewModel<RouteViewModel>> GetAll();
        Task<Response> CreateRoute(CreateRouteModel model);
        Task<RouteViewModel> GetRouteById(Guid id);
    }
}
