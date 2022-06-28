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
            var isExistCode = await _unitOfWork.PriceOfBookingServiceRepository.Query().AnyAsync(x => x.VehicleTypeId == model.VehicleTypeId);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Giá đã tồn tại!"
                };
            }

            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            var price = new PriceOfBookingService()
            {
                PriceOfBookingServiceId = Guid.NewGuid(),
                FixedDistance = model.FixedDistance.Value,
                FixedPrice = model.FixedPrice.Value,
                PricePerKilometer = model.PricePerKilometer.Value,
                VehicleTypeId = model.VehicleTypeId.Value,
                Status = 1
            };

            await _unitOfWork.PriceOfBookingServiceRepository.Add(price);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới giá thành công!"
            };
        }

        public async Task<Response> DeletePrice(Guid id)
        {
            var entity = await _unitOfWork.PriceOfBookingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.PriceOfBookingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PriceOfBookingServiceViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.PriceOfBookingServiceRepository.GetById(id);
            var model = entity.AsPriceOfBookingServiceViewModel();
            var vehicleType = await _unitOfWork.VehicleTypeRepository.GetById(model.VehicleTypeId);
            model.VehicleLabel = vehicleType.Label;
            model.VehicleSeats = vehicleType.Seats;
            model.VehicleFuel = vehicleType.Fuel;
            return model;

        }

        public async Task<List<PriceOfBookingServiceViewModel>> Search(PriceBookingServiceSearchModel model)
        {
            var entity = await _unitOfWork.PriceOfBookingServiceRepository.Query()
                            .Where(x => model.VehicleTypeId == null || x.VehicleTypeId.Equals(model.VehicleTypeId.Value))
                            .Where(x => model.FixedPrice == null || x.FixedPrice == model.FixedPrice.Value)
                            .Where(x => model.FixedDistance == null || x.FixedDistance == model.FixedDistance.Value)
                            .Where(x => model.PricePerKilometer == null || x.PricePerKilometer == model.PricePerKilometer.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPriceOfBookingServiceViewModel())
                            .ToListAsync();
            foreach (PriceOfBookingServiceViewModel x in entity)
            {
                var vehicleType = await _unitOfWork.VehicleTypeRepository.GetById(x.VehicleTypeId);
                x.VehicleLabel = vehicleType.Label;
                x.VehicleSeats = vehicleType.Seats;
                x.VehicleFuel = vehicleType.Fuel;
            }
            return entity;

        }

        public async Task<Response> UpdatePrice(Guid id, UpdatePriceBookingServiceModel model)
        {
            var entity = await _unitOfWork.PriceOfBookingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            if (model.VehicleTypeId != null && entity.VehicleTypeId != model.VehicleTypeId.Value)
            {
                var isExistCode = await _unitOfWork.PriceOfBookingServiceRepository.Query().AnyAsync(x => x.VehicleTypeId == model.VehicleTypeId);
                if (isExistCode)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Giá đã tồn tại!"
                    };
                }
            }

            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            entity.VehicleTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleTypeId, model.VehicleTypeId);
            entity.FixedDistance = UpdateTypeOfNotNullAbleObject<decimal>(entity.FixedDistance, model.FixedDistance);
            entity.FixedPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.FixedPrice, model.FixedPrice);
            entity.PricePerKilometer = UpdateTypeOfNotNullAbleObject<decimal>(entity.PricePerKilometer, model.PricePerKilometer);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.PriceOfBookingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        private async Task<Response> CheckValidationData(PriceBookingModel model)
        {
            if (model.VehicleTypeId != null)
            {
                var vehicleType = await _unitOfWork.VehicleTypeRepository.GetById(model.VehicleTypeId.Value);
                if (vehicleType == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Loại xe không tồn tại!"
                    };
                }
                else if (vehicleType.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với loại xe đã bị vô hiệu hóa"
                    };
                }
            }
            return new()
            {
                StatusCode = 0,
            };
        }
    }
}