using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface IRouteManagementService
    {
        Task<SearchResultViewModel<RouteViewModel>> GetAll();
    }
}
