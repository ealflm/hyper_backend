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
        // "rate value" for calculating price of bus service from base price value
        private readonly double[] rateArray = { 0.25, 0.5, 0.75, 1 };

        public PriceBusServiceConfigService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        // public async Task<Response> CreatePrice(CreatePriceBusServiceModel model)
        // {
        //     var price = new PriceOfBusService()
        //     {
        //         PriceOfBusServiceId = Guid.NewGuid(),
        //         MinDistance = model.MinDistance,
        //         MaxDistance = model.MaxDistance,
        //         Price = model.Price,
        //         MinStation = model.MinStation,
        //         MaxStation = model.MaxStation,
        //         Mode = model.Mode,
        //         Status = 1
        //     };

        //     await _unitOfWork.PriceOfBusServiceRepository.Add(price);
        //     await _unitOfWork.SaveChangesAsync();

        //     return new()
        //     {
        //         StatusCode = 201,
        //         Message = "Tạo mới khuyến mãi thành công!"
        //     };
        // }

        public async Task<Response> DeletePrice(Guid id)
        {
            var entity = await _unitOfWork.PriceOfBusServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.PriceOfBusServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PriceOfBusServiceViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.PriceOfBusServiceRepository.GetById(id);
            return entity.AsPriceOfBusServiceViewModel();

        }

        public async Task<List<PriceOfBusServiceViewModel>> Search(PriceBusServiceSearchModel model)
        {
            var entity = await _unitOfWork.PriceOfBusServiceRepository.Query()
                            .Where(x => model.BasePriceBusServiceId == null || x.BasePriceId == model.BasePriceBusServiceId.Value)
                            .Where(x => model.MinDistance == null || x.MinDistance == model.MinDistance.Value)
                            .Where(x => model.MaxDistance == null || x.MaxDistance == model.MaxDistance.Value)
                            .Where(x => model.MinStation == null || x.MinStation == model.MinStation.Value)
                            .Where(x => model.MaxStation == null || x.MaxStation == model.MaxStation.Value)
                            .Where(x => model.Mode == null || x.Mode.Contains(model.Mode))
                            .Where(x => model.Price == null || x.Price == model.Price.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPriceOfBusServiceViewModel())
                            .ToListAsync();
            foreach (var p in entity)
            {
                var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(p.BasePriceId);
                p.BaseMinDistance = basePrice.MinDistance;
                p.BaseMaxDistance = basePrice.MaxDistance;
            }
            return entity;
        }

        // public async Task<Response> UpdatePrice(Guid id, UpdatePriceBusServiceModel model)
        // {
        //     var entity = await _unitOfWork.PriceOfBusServiceRepository.GetById(id);
        //     if (entity == null)
        //     {
        //         return new()
        //         {
        //             StatusCode = 404,
        //             Message = "Không tìm thấy!"
        //         };
        //     }

        //     entity.MinDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinDistance, model.MinDistance);
        //     entity.MaxDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxDistance, model.MaxDistance);
        //     entity.MinStation = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinStation, model.MinStation);
        //     entity.MaxStation = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxStation, model.MaxStation);
        //     entity.Price = UpdateTypeOfNotNullAbleObject<decimal>(entity.Price, model.Price);
        //     entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
        //     entity.Mode = UpdateTypeOfNullAbleObject<string>(entity.Mode, model.Mode);
        //     _unitOfWork.PriceOfBusServiceRepository.Update(entity);
        //     await _unitOfWork.SaveChangesAsync();
        //     return new()
        //     {
        //         StatusCode = 201,
        //         Message = "Cập nhật thành công!"
        //     };
        // }
    }
}