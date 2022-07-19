using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PublishYearManagementService : BaseService, IPublishYearManagementService
    {
        public PublishYearManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> Add(CreatePublishYearModel model)
        {
            var isExistCode = await _unitOfWork.PublishYearRepository.Query().AnyAsync(x => x.Name == model.Name);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Năm sản xuất đã tồn tại!"
                };
            }

            var entity = new PublishYear()
            {
                PublishYearId = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Status = 1
            };
            await _unitOfWork.PublishYearRepository.Add(entity);

            // generate default price with value 0
            var categoriesList = await _unitOfWork.CategoryRepository
                                .Query()
                                .Select(x => x.AsCategoryViewModel())
                                .ToListAsync();

            foreach (var p in categoriesList)
            {
                var newRecord = new PriceOfRentingService()
                {
                    PriceOfRentingServiceId = Guid.NewGuid(),
                    CategoryId = p.Id,
                    PublishYearId = entity.PublishYearId,
                    MinTime = 0,
                    MaxTime = 0,
                    PricePerHour = 0,
                    FixedPrice = 0,
                    WeekendPrice = 0,
                    HolidayPrice = 0,
                    Status = 1
                };

                await _unitOfWork.PriceOfRentingServiceRepository.Add(newRecord);
            }

            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới năm sản xuất thành công!"
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.PublishYearRepository.GetById(id);
            if (entity is null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            var result = await CheckReferenceToOther(id);
            if (result.StatusCode != 0)
            {
                return result;
            }

            entity.Status = 0;
            _unitOfWork.PublishYearRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PublishYearViewModel> Get(Guid id)
        {
            var entity = await _unitOfWork.PublishYearRepository.GetById(id);
            if (entity == null)
            {
                return null;
            }
            return entity.AsPublishYearViewModel();
        }

        public async Task<SearchResultViewModel<PublishYearViewModel>> GetAll(PublishYearSearchModel model)
        {
            var publishYears = await _unitOfWork.PublishYearRepository.Query()
                .Where(x => model.Name == null || model.Name.Contains(model.Name))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .Select(x => x.AsPublishYearViewModel())
                .ToListAsync();
            SearchResultViewModel<PublishYearViewModel> result = null;
            result = new SearchResultViewModel<PublishYearViewModel>()
            {
                Items = publishYears,
                PageSize = 1,
                TotalItems = publishYears.Count
            };
            return result;
        }

        public async Task<Response> Update(Guid id, UpdatePublishYearModel model)
        {
            var entity = await _unitOfWork.PublishYearRepository.GetById(id);
            if (entity is null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }

            if (entity.Name != model.Name)
            {
                var isExistedCode = await _unitOfWork.PublishYearRepository.Query().AnyAsync(x => x.Name.Equals(model.Name));
                if (isExistedCode)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Năm sản xuất đã tồn tại!"
                    };
                }
            }
            else
            {
                entity.Name = model.Name;
            }

            if (model.Status == null)
            {
                entity.Status = entity.Status;
            }
            else if (model.Status.Value == 0)
            {
                var result = await CheckReferenceToOther(id);
                if (result.StatusCode != 0)
                {
                    return result;
                }
                entity.Status = 0;
            }
            else
            {
                entity.Status = model.Status.Value;
            }

            entity.Name = UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
            entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
            entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
            // entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.PublishYearRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        private async Task<Response> CheckReferenceToOther(Guid id)
        {
            var obj = new Response()
            {
                StatusCode = 400,
                Message = "Dữ liệu đã được tham chiếu, bạn không thể xóa dữ liệu này"
            };

            var checkExistedReferenceToPriceOfBookingService = await _unitOfWork.PriceOfRentingServiceRepository
                                                                    .Query()
                                                                    .AnyAsync(x => x.PublishYearId == id && x.Status == 1);
            if (checkExistedReferenceToPriceOfBookingService)
            {
                return obj;
            }

            return new()
            {
                StatusCode = 0
            };
        }
    }
}
