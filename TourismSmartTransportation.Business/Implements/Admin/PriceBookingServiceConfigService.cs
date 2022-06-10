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
using TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBookingServiceViewModel;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PriceBookingServiceConfigService : BaseService, IPriceBookingServiceConfig
    {
        public PriceBookingServiceConfigService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreatePrice(CreatePriceBookingServiceModel model)
        {
            var isExistCode = await _unitOfWork.PriceListOfBookingServiceRepository.Query().AnyAsync(x => x.VehicleTypeId == model.VehicleTypeId);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Giá đã tồn tại!"
                };
            }
            var price = new PriceListOfBookingService()
            {
                Id = Guid.NewGuid(),
                FixedDistance= model.FixedDistance,
                FixedPrice= model.FixedPrice,
                PricePerKilometer= model.PricePerKilometer,
                VehicleTypeId= model.VehicleTypeId,
                Status = 1
            };

            await _unitOfWork.PriceListOfBookingServiceRepository.Add(price);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới giá thành công!"
            };
        }

        public async Task<Response> DeletePrice(Guid id)
        {
            var entity = await _unitOfWork.PriceListOfBookingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.PriceListOfBookingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PriceBookingServiceViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.PriceListOfBookingServiceRepository.GetById(id);
            return entity.AsPriceListOfBookingService();

        }

        public async Task<List<PriceBookingServiceViewModel>> Search(PriceBookingServiceSearchModel model)
        {
            var entity = await _unitOfWork.PriceListOfBookingServiceRepository.Query()
                            .Where(x => model.VehicleTypeId == null || x.VehicleTypeId.Equals(model.VehicleTypeId.Value))
                            .Where(x => model.FixedPrice == null || x.FixedPrice == model.FixedPrice.Value)
                            .Where(x => model.FixedDistance == null || x.FixedDistance == model.FixedDistance.Value)
                            .Where(x => model.PricePerKilometer == null || x.PricePerKilometer == model.PricePerKilometer.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPriceListOfBookingService())
                            .ToListAsync();
            return entity;

        }

        public async Task<Response> UpdatePrice(Guid id, UpdatePriceBookingServiceModel model)
        {
            var isExistCode = await _unitOfWork.PriceListOfBookingServiceRepository.Query().AnyAsync(x => x.VehicleTypeId == model.VehicleTypeId.Value);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Giá đã tồn tại!"
                };
            }
            var entity = await _unitOfWork.PriceListOfBookingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.FixedDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.FixedDistance, model.FixedDistance);
            entity.FixedPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.FixedPrice, model.FixedPrice);
            entity.PricePerKilometer = UpdateTypeOfNotNullAbleObject<decimal>(entity.PricePerKilometer, model.PricePerKilometer);
            entity.VehicleTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleTypeId, model.VehicleTypeId);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.PriceListOfBookingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}