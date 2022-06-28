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
            var isExistCode = await _unitOfWork.PriceOfRentingServiceRepository.Query().AnyAsync(x => x.CategoryId == model.CategoryId && x.PublishYearId == model.PublishYearId);
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

            var price = new PriceOfRentingService()
            {
                PriceOfRentingServiceId = Guid.NewGuid(),
                CategoryId = model.CategoryId.Value,
                FixedPrice = model.FixedPrice.Value,
                HolidayPrice = model.HolidayPrice.Value,
                MaxTime = model.MaxTime.Value,
                MinTime = model.MinTime.Value,
                PricePerHour = model.PricePerHour.Value,
                PublishYearId = model.PublishYearId.Value,
                WeekendPrice = model.WeekendPrice.Value,
                Status = 1
            };

            await _unitOfWork.PriceOfRentingServiceRepository.Add(price);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới giá thành công!"
            };
        }

        public async Task<Response> DeletePrice(Guid id)
        {
            var entity = await _unitOfWork.PriceOfRentingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.PriceOfRentingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PriceOfRentingServiceViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.PriceOfRentingServiceRepository.GetById(id);
            return entity.AsPriceOfRentingService();

        }

        public async Task<List<PriceOfRentingServiceViewModel>> Search(PriceRentingServiceSearchModel model)
        {
            var entity = await _unitOfWork.PriceOfRentingServiceRepository.Query()
                            .Where(x => model.CategoryId == null || x.CategoryId.Equals(model.CategoryId))
                            .Where(x => model.PublishYearId == null || x.PublishYearId.Equals(model.PublishYearId))
                            .Where(x => model.FixedPrice == null || x.FixedPrice == model.FixedPrice.Value)
                            .Where(x => model.HolidayPrice == null || x.HolidayPrice == model.HolidayPrice.Value)
                            .Where(x => model.MaxTime == null || x.MaxTime == model.MaxTime.Value)
                            .Where(x => model.MinTime == null || x.MinTime == model.MinTime.Value)
                            .Where(x => model.PricePerHour == null || x.PricePerHour == model.PricePerHour.Value)
                            .Where(x => model.WeekendPrice == null || x.WeekendPrice == model.WeekendPrice.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPriceOfRentingService())
                            .ToListAsync();
            foreach (PriceOfRentingServiceViewModel x in entity)
            {
                x.CategoryName = (await _unitOfWork.CategoryRepository.GetById(x.CategoryId)).Name;
                x.PublishYearName = (await _unitOfWork.PublishYearRepository.GetById(x.PublishYearId)).Name;
            }
            return entity;

        }

        public async Task<Response> UpdatePrice(Guid id, UpdatePriceRentingServiceModel model)
        {
            var entity = await _unitOfWork.PriceOfRentingServiceRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            if (model.CategoryId != null && model.CategoryId.Value != entity.CategoryId
                && model.PublishYearId != null && model.PublishYearId.Value != entity.PublishYearId)
            {
                var isExistCode = await _unitOfWork.PriceOfRentingServiceRepository.Query().AnyAsync(x => x.CategoryId == model.CategoryId.Value && x.PublishYearId == model.PublishYearId.Value);
                if (isExistCode)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Giá đã tồn tại!"
                    };
                }
            }
            else if (model.CategoryId != null && model.CategoryId.Value != entity.CategoryId)
            {
                var isExistCode = await _unitOfWork.PriceOfRentingServiceRepository.Query().AnyAsync(x => x.CategoryId == model.CategoryId.Value && x.PublishYearId == entity.PublishYearId);
                if (isExistCode)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Giá đã tồn tại!"
                    };
                }
            }
            else if (model.PublishYearId != null && model.PublishYearId.Value != entity.PublishYearId)
            {
                var isExistCode = await _unitOfWork.PriceOfRentingServiceRepository.Query().AnyAsync(x => x.CategoryId == entity.CategoryId && x.PublishYearId == model.PublishYearId.Value);
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

            entity.HolidayPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.HolidayPrice, model.HolidayPrice);
            entity.FixedPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.FixedPrice, model.FixedPrice);
            entity.PricePerHour = UpdateTypeOfNotNullAbleObject<decimal>(entity.PricePerHour, model.PricePerHour);
            entity.WeekendPrice = UpdateTypeOfNotNullAbleObject<decimal>(entity.WeekendPrice, model.WeekendPrice);
            entity.MaxTime = UpdateTypeOfNotNullAbleObject<decimal>(entity.MaxTime, model.MaxTime);
            entity.MinTime = UpdateTypeOfNotNullAbleObject<decimal>(entity.MinTime, model.MinTime);
            entity.CategoryId = UpdateTypeOfNotNullAbleObject<Guid>(entity.CategoryId, model.CategoryId);
            entity.PublishYearId = UpdateTypeOfNotNullAbleObject<Guid>(entity.PublishYearId, model.PublishYearId);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.PriceOfRentingServiceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        private async Task<Response> CheckValidationData(PriceRentingModel model)
        {
            // Check category
            if (model.CategoryId != null)
            {
                var category = await _unitOfWork.CategoryRepository.GetById(model.CategoryId.Value);
                if (category == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Hạng xe không tồn tại"
                    };
                }
                else if (category.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với hạng xe đã bị vô hiệu hóa"
                    };
                }
            }

            // Check publish year
            if (model.PublishYearId != null)
            {
                var publicYear = await _unitOfWork.PublishYearRepository.GetById(model.PublishYearId.Value);
                if (publicYear == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Năm sản xuất không tồn tại"
                    };
                }
                else if (publicYear.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với năm sản xuất đã bị vô hiệu hóa"
                    };
                }
            }


            return new()
            {
                StatusCode = 0
            };
        }
    }
}