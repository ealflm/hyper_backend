using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class RouteManagementService : BaseService, IRouteManagementService
    {
        public RouteManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<SearchResultViewModel<RouteViewModel>> GetAll()
        {
            var routes = await _unitOfWork.RouteRepository.Query()
               .Select(x => x.AsRouteViewModel())
               .ToListAsync();
            foreach(RouteViewModel x in routes)
            {
                var stationRouteList = await _unitOfWork.StationRouteRepository.Query().Where(y => y.RouteId.Equals(x.Id)).ToListAsync();
                x.StationList = new List<ViewModel.Admin.StationManagement.StationViewModel>();
                foreach(StationRoute y in stationRouteList)
                {
                    var station = await _unitOfWork.StationRepository.GetById(y.StationId);
                    x.StationList.Add(station.AsStationViewModel());
                }
            }
            
            SearchResultViewModel<RouteViewModel> result = null;
            result = new SearchResultViewModel<RouteViewModel>()
            {
                Items = routes,
                PageSize = 1,
                TotalItems = routes.Count
            };
            return result;

        }
    }
}
