using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBusServiceViewModel;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PriceBusServiceConfigService : BaseService, IPriceBusServiceConfig
    {
        public PriceBusServiceConfigService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreatePrice(CreatePriceBusServiceModel model)
        {
            var price = new PriceListOfBusService()
            {
                Id = Guid.NewGuid(),
                MinRouteDistance = model.MinRouteDistance,
                MaxRouteDistance = model.MaxRouteDistance,
                MinDistance = model.MinDistance,
                MaxDistance = model.MaxDistance,
                Price = model.Price,
                MinStation = model.MinStation,
                MaxStation = model.MaxStation,
                Mode = model.Mode,
                Status = 1
            };

            await _unitOfWork.PriceListOfBusServiceRepository.Add(price);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới khuyến mãi thành công!"
            };
        }

        public async Task<Response> DeletePrice(Guid id)
        {
            var entity = await _unitOfWork.PriceListOfBusServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.PriceListOfBusServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PriceBusServiceViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.PriceListOfBusServiceRepository.GetById(id);
            return entity.AsPriceListOfBusService();

        }

        public async Task<List<PriceBusServiceViewModel>> Search(PriceBusServiceSearchModel model)
        {
            var entity = await _unitOfWork.PriceListOfBusServiceRepository.Query()
                            .Where(x => model.MinRouteDistance == null || x.MinRouteDistance == model.MinRouteDistance.Value)
                            .Where(x => model.MaxRouteDistance == null || x.MaxRouteDistance == model.MaxRouteDistance.Value)
                            .Where(x => model.MinDistance == null || x.MinDistance == model.MinDistance.Value)
                            .Where(x => model.MaxDistance == null || x.MaxDistance == model.MaxDistance.Value)
                            .Where(x => model.MinStation == null || x.MinStation == model.MinStation.Value)
                            .Where(x => model.MaxStation == null || x.MaxStation == model.MaxStation.Value)
                            .Where(x => model.Mode == null || x.Mode == model.Mode)
                            .Where(x => model.Price == null || x.Price == model.Price.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPriceListOfBusService())
                            .ToListAsync();
            return entity;

        }

        public async Task<Response> UpdatePrice(Guid id, UpdatePriceBusServiceModel model)
        {
            var entity = await _unitOfWork.PriceListOfBusServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            entity.MinRouteDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinRouteDistance, model.MinRouteDistance);
            entity.MaxRouteDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxRouteDistance, model.MaxRouteDistance);
            entity.MinDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinDistance, model.MinDistance);
            entity.MaxDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxDistance, model.MaxDistance);
            entity.MinStation = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinStation, model.MinStation);
            entity.MaxStation = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxStation, model.MaxStation);
            entity.Price = UpdateTypeOfNotNullAbleObject<decimal>(entity.Price, model.Price);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            entity.Mode = UpdateTypeOfNullAbleObject<string>(entity.Mode, model.Mode);
            _unitOfWork.PriceListOfBusServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}