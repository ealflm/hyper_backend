using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
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

        public async Task<Response> CreateRoute(CreateRouteModel model)
        {
            var entity = new Route()
            {
                RouteId = Guid.NewGuid(),
                PartnerId = model.PartnerId,
                Name = model.Name,
                TotalStation = model.TotalStation,
                Distance = model.Distance,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Status = 1
            };
            await _unitOfWork.RouteRepository.Add(entity);
            foreach (var p in model.StationList)
            {
                p.RouteId = entity.RouteId;
                await _unitOfWork.StationRouteRepository.Add(p.AsStationRouteData());
            }
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo tuyến đường thành công!"
            };
        }

        public async Task<SearchResultViewModel<RouteViewModel>> GetAll(RouteSearchModel model)
        {
            var routes = await _unitOfWork.RouteRepository.Query()
                        .Where(x => model.PartnerId == null || x.PartnerId == model.PartnerId.Value)
                        .Where(x => model.Name == null || x.Name.Contains(model.Name))
                        .Select(x => x.AsRouteViewModel())
                        .ToListAsync();
            foreach (RouteViewModel x in routes)
            {
                var stationRouteList = await _unitOfWork.StationRouteRepository.Query().Where(y => y.RouteId.Equals(x.Id)).ToListAsync();
                x.StationList = new List<ViewModel.Admin.StationManagement.StationViewModel>();
                foreach (StationRoute y in stationRouteList)
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

        public async Task<RouteViewModel> GetRouteById(Guid id)
        {
            var entity = await _unitOfWork.RouteRepository.GetById(id);
            if (entity == null)
            {
                return null;
            }
            var stationRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(entity.RouteId)).ToListAsync();
            var route = entity.AsRouteViewModel();
            route.StationList = new List<ViewModel.Admin.StationManagement.StationViewModel>();
            foreach (StationRoute s in stationRouteList)
            {
                var station = await _unitOfWork.StationRepository.GetById(s.StationId);
                route.StationList.Add(station.AsStationViewModel());
            }
            return route;
        }
    }
}
