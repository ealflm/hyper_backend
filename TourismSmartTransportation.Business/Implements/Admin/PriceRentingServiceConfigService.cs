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
using TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceRentingServiceViewModel;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PriceRentingServiceConfigService : BaseService, IPriceRentingServiceConfig
    {
        public PriceRentingServiceConfigService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreatePrice(CreatePriceRentingServiceModel model)
        {
            var isExistCode = await _unitOfWork.PriceListOfRentingServiceRepository.Query().AnyAsync(x => x.CategoryId == model.CategoryId && x.PublishYearId == model.PublishYearId);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Giá đã tồn tại!"
                };
            }
            var price = new PriceListOfRentingService()
            {
                Id = Guid.NewGuid(),
                CategoryId= model.CategoryId,
                FixedPrice= model.FixedPrice,
                HolidayPrice= model.HolidayPrice,
                MaxTime= model.MaxTime,
                MinTime= model.MinTime,
                PricePerHour= model.PricePerHour,
                PublishYearId= model.PublishYearId,
                WeekendPrice= model.WeekendPrice,
                Status = 1
            };

            await _unitOfWork.PriceListOfRentingServiceRepository.Add(price);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới giá thành công!"
            };
        }

        public async Task<Response> DeletePrice(Guid id)
        {
            var entity = await _unitOfWork.PriceListOfRentingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.PriceListOfRentingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PriceRentingServiceViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.PriceListOfRentingServiceRepository.GetById(id);
            return entity.AsPriceListOfRentingService();

        }

        public async Task<List<PriceRentingServiceViewModel>> Search(PriceRentingServiceSearchModel model)
        {
            var entity = await _unitOfWork.PriceListOfRentingServiceRepository.Query()
                            .Where(x => model.CategoryId == null || x.CategoryId.Equals(model.CategoryId))
                            .Where(x => model.PublishYearId == null || x.PublishYearId.Equals(model.PublishYearId))
                            .Where(x => model.FixedPrice == null || x.FixedPrice == model.FixedPrice.Value)
                            .Where(x => model.HolidayPrice == null || x.HolidayPrice == model.HolidayPrice.Value)
                            .Where(x => model.MaxTime == null || x.MaxTime == model.MaxTime.Value)
                            .Where(x => model.MinTime == null || x.MinTime == model.MinTime.Value)
                            .Where(x => model.PricePerHour == null || x.PricePerHour == model.PricePerHour.Value)
                            .Where(x => model.WeekendPrice == null || x.WeekendPrice == model.WeekendPrice.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPriceListOfRentingService())
                            .ToListAsync();
            foreach(PriceRentingServiceViewModel x  in entity)
            {
                x.CategoryName = (await _unitOfWork.CategoryRepository.GetById(x.CategoryId)).Name;
                x.PublishYearName = (await _unitOfWork.PublishYearRepository.GetById(x.PublishYearId)).Name;
            }
            return entity;

        }

        public async Task<Response> UpdatePrice(Guid id, UpdatePriceRentingServiceModel model)
        {
            var isExistCode = await _unitOfWork.PriceListOfRentingServiceRepository.Query().AnyAsync(x => x.CategoryId == model.CategoryId && x.PublishYearId == model.PublishYearId);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Giá đã tồn tại!"
                };
            }
            var entity = await _unitOfWork.PriceListOfRentingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.HolidayPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.HolidayPrice, model.HolidayPrice);
            entity.FixedPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.FixedPrice, model.FixedPrice);
            entity.PricePerHour = UpdateTypeOfNotNullAbleObject<decimal>(entity.PricePerHour, model.PricePerHour);
            entity.WeekendPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.WeekendPrice, model.WeekendPrice);
            entity.MaxTime = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxTime, model.MaxTime);
            entity.MinTime = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinTime, model.MinTime);
            entity.CategoryId = UpdateTypeOfNotNullAbleObject<Guid>(entity.CategoryId, model.CategoryId);
            entity.PublishYearId = UpdateTypeOfNotNullAbleObject<Guid>(entity.PublishYearId, model.PublishYearId);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.PriceListOfRentingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}