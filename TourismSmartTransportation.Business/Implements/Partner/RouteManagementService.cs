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
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBusServiceViewModel;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class RouteManagementService : BaseService, IRouteManagementService
    {
        private readonly double[] rateArray = { 0.25, 0.5, 0.75, 1 };

        public RouteManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreateRoute(CreateRouteModel model)
        {
            HashSet<Guid> checkSet = new HashSet<Guid>();
            foreach(var x in model.StationList)
            {
                var routes = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(x.StationId)).ToListAsync();
                foreach(var y in routes)
                {
                    if (!checkSet.Add(y.RouteId))
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = "Tuyến đã tồn tại"
                        };
                    }
                }
            }

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

            // add the stations for the route
            foreach (var p in model.StationList)
            {
                p.RouteId = entity.RouteId;
                await _unitOfWork.StationRouteRepository.Add(p.AsStationRouteData());
            }

            var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository
                            .Query()
                            .Where(x => x.MinDistance <= entity.Distance && entity.Distance <= x.MaxDistance)
                            .OrderBy(x => x.MinDistance)
                            .Select(x => x.AsBasePriceOfBusServiceViewModel())
                            .FirstOrDefaultAsync();

            if (basePrice == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Cần tạo giá cơ sở trước khi thực hiện các thao tác tạo tuyến đường."
                };
            }

            List<PriceOfBusServiceViewModel> list = new List<PriceOfBusServiceViewModel>();

            // Create price follow by number of stations
            for (int i = 0; i < rateArray.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            var price = new PriceOfBusService()
                            {
                                PriceOfBusServiceId = Guid.NewGuid(),
                                BasePriceId = basePrice.BasePriceOfBusServiceId,
                                MinDistance = (basePrice.MaxDistance - basePrice.MinDistance) * (decimal)rateArray[i] + basePrice.MinDistance,
                                MaxDistance = (basePrice.MaxDistance - basePrice.MinDistance) * (decimal)rateArray[i + 1] + basePrice.MinDistance,
                                Price = basePrice.Price * (decimal)rateArray[i],
                                MinStation = 1,
                                MaxStation = Math.Ceiling(entity.TotalStation * (decimal)rateArray[i + 1]),
                                Mode = "station",
                                Status = 1,
                            };
                            await _unitOfWork.PriceOfBusServiceRepository.Add(price);
                            list.Add(price.AsPriceOfBusServiceViewModel());
                            break;
                        }
                    default:
                        {
                            int index = i == rateArray.Count() - 1 ? i : i + 1;
                            var price = new PriceOfBusService()
                            {
                                PriceOfBusServiceId = Guid.NewGuid(),
                                BasePriceId = basePrice.BasePriceOfBusServiceId,
                                MinDistance = (basePrice.MaxDistance - basePrice.MinDistance) * (decimal)rateArray[i] + basePrice.MinDistance,
                                MaxDistance = (basePrice.MaxDistance - basePrice.MinDistance) * (decimal)rateArray[index] + basePrice.MinDistance,
                                Price = basePrice.Price * (decimal)rateArray[i],
                                MinStation = Math.Ceiling(entity.TotalStation * (decimal)rateArray[i]),
                                MaxStation = Math.Ceiling(entity.TotalStation * (decimal)rateArray[index]),
                                Mode = "station",
                                Status = 1,
                            };
                            await _unitOfWork.PriceOfBusServiceRepository.Add(price);
                            list.Add(price.AsPriceOfBusServiceViewModel());
                            break;
                        };
                }
            }

            // Get list price to assign for the route
            var priceOfBusServiceList = await _unitOfWork.PriceOfBusServiceRepository
                                        .Query()
                                        .Where(x => x.BasePriceId == basePrice.BasePriceOfBusServiceId)
                                        .Select(x => x.AsPriceOfBusServiceViewModel())
                                        .ToListAsync();
            list.AddRange(priceOfBusServiceList);

            foreach (var item in list)
            {
                var routePrice = new RoutePriceBusing()
                {
                    RouteId = entity.RouteId,
                    PriceBusingId = item.Id,
                };
                await _unitOfWork.RoutePriceBusingRepository.Add(routePrice);
            }
            await _unitOfWork.SaveChangesAsync();
            var stationList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(entity.RouteId)).ToListAsync();
            var checkRoute = new HashSet<Guid>();
            foreach(StationRoute x in stationList)
            {
                var routeList = await _unitOfWork.StationRouteRepository.Query().Where(k => k.StationId.Equals(x.StationId)).ToListAsync();
                foreach(StationRoute y in routeList)
                {
                    if (!y.RouteId.Equals(x.RouteId) && checkRoute.Add(y.RouteId))
                    {

                        var linkRoute = new LinkRoute()
                        {
                            FirstRouteId = x.RouteId,
                            SecondRouteId = y.RouteId,
                            StationId = x.StationId
                        };
                        await _unitOfWork.LinkRouteRepository.Add(linkRoute);
                    }
                }
                var linkStationList = await _unitOfWork.LinkStationRepository.Query().Where(k => k.FirstStationId.Equals(x.StationId)).ToListAsync();
                foreach(LinkStation y in linkStationList)
                {
                    var LinkStationRouteList= await _unitOfWork.StationRouteRepository.Query().Where(k => k.StationId.Equals(y.SecondStationId)).ToListAsync();
                    foreach(StationRoute z in LinkStationRouteList)
                    {
                        if (!z.RouteId.Equals(x.RouteId) && checkRoute.Add(z.RouteId))
                        {

                            var linkRoute = new LinkRoute()
                            {
                                FirstRouteId = x.RouteId,
                                SecondRouteId = z.RouteId,
                                LinkStationId= y.LinkStationId
                            };
                            await _unitOfWork.LinkRouteRepository.Add(linkRoute);
                        }
                    }
                }
                linkStationList = await _unitOfWork.LinkStationRepository.Query().Where(k => k.SecondStationId.Equals(x.StationId)).ToListAsync();
                foreach (LinkStation y in linkStationList)
                {
                    var LinkStationRouteList = await _unitOfWork.StationRouteRepository.Query().Where(k => k.StationId.Equals(y.FirstStationId)).ToListAsync();
                    foreach (StationRoute z in LinkStationRouteList)
                    {
                        if (!z.RouteId.Equals(x.RouteId) && checkRoute.Add(z.RouteId))
                        {

                            var linkRoute = new LinkRoute()
                            {
                                FirstRouteId = x.RouteId,
                                SecondRouteId = z.RouteId,
                                LinkStationId = y.LinkStationId
                            };
                            await _unitOfWork.LinkRouteRepository.Add(linkRoute);
                        }
                    }
                }
            }
            PartnerRoute partnerRoute = new PartnerRoute()
            {
                PartnerId = model.PartnerId,
                RouteId = entity.RouteId
            };
            await _unitOfWork.PartnerRouteRepository.Add(partnerRoute);
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
                        .Where(x => model.Name == null || x.Name.Contains(model.Name))
                        .Where(x=> x.Status==1)
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
            List<RouteViewModel> items = new List<RouteViewModel>();
            if (model.PartnerId != null)
            {
                foreach (var x in routes)
                {
                    var partnerRoutes = await _unitOfWork.PartnerRouteRepository.Query().Where(y => y.PartnerId.Equals(model.PartnerId) && y.RouteId.Equals(x.Id)).FirstOrDefaultAsync();
                    if (partnerRoutes != null)
                    {
                        items.Add(x);
                    }
                }
            }
            else
            {
                items = routes;
            }
            SearchResultViewModel<RouteViewModel> result = null;
            result = new SearchResultViewModel<RouteViewModel>()
            {
                Items = items,
                PageSize = 1,
                TotalItems = items.Count
            };
            return result;

        }

        public async Task<SearchResultViewModel<RouteViewModel>> GetRouteAlready(Guid partnerId)
        {
            var routes = await _unitOfWork.RouteRepository.Query().ToListAsync();
            List<RouteViewModel> items = new List<RouteViewModel>();
            foreach(var x in routes)
            {
                var partnerRoutes = await _unitOfWork.PartnerRouteRepository.Query().Where(y => y.PartnerId.Equals(partnerId) && y.RouteId.Equals(x.RouteId)).FirstOrDefaultAsync();
                if(partnerRoutes== null)
                {
                    items.Add(x.AsRouteViewModel());
                }
            }

            SearchResultViewModel<RouteViewModel> result = null;
            result = new SearchResultViewModel<RouteViewModel>()
            {
                Items = items,
                PageSize = 1,
                TotalItems = items.Count
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
            var stationRouteList = await _unitOfWork.StationRouteRepository
                                .Query()
                                .Where(x => x.RouteId.Equals(entity.RouteId))
                                .OrderBy(x => x.OrderNumber)
                                .ToListAsync();
            var route = entity.AsRouteViewModel();
            route.StationList = new List<ViewModel.Admin.StationManagement.StationViewModel>();
            foreach (StationRoute s in stationRouteList)
            {
                var station = await _unitOfWork.StationRepository.GetById(s.StationId);
                route.StationList.Add(station.AsStationViewModel());
            }
            return route;
        }

        public async Task<Response> AddRouteToPartner(Guid routeId, Guid partnerId)
        {
            PartnerRoute partnerRoute = new PartnerRoute()
            {
                PartnerId = partnerId,
                RouteId = routeId
            };
            await _unitOfWork.PartnerRouteRepository.Add(partnerRoute);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode=200,
                 Message="Thêm tuyến thành công"
            };
        }
    }
}
