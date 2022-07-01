using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CategoryManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CategoryManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class CategoryManagementService : BaseService, ICategoryManagementService
    {
        public CategoryManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> Add(CreateCategoryModel model)
        {
            var isExistCode = await _unitOfWork.CategoryRepository.Query().AnyAsync(x => x.Name == model.Name);
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Hạng xe đã tồn tại!"
                };
            }

            var entity = new Category()
            {
                CategoryId = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Status = 1
            };
            await _unitOfWork.CategoryRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới hạng xe thành công!"
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.CategoryRepository.GetById(id);
            if (entity == null)
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
            _unitOfWork.CategoryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<CategoryViewModel> Get(Guid id)
        {
            var entity = await _unitOfWork.CategoryRepository.GetById(id);
            if (entity == null)
            {
                return null;
            }
            return entity.AsCategoryViewModel();
        }

        public async Task<SearchResultViewModel<CategoryViewModel>> GetAll(CategorySearchModel model)
        {
            var category = await _unitOfWork.CategoryRepository.Query()
                .Where(x => model.Name == null || x.Name.Contains(model.Name))
                .Where(x => model.Description == null || x.Description.Contains(model.Description))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .Select(x => x.AsCategoryViewModel())
                .ToListAsync();
            SearchResultViewModel<CategoryViewModel> result = null;
            result = new SearchResultViewModel<CategoryViewModel>()
            {
                Items = category,
                PageSize = 1,
                TotalItems = category.Count
            };
            return result;
        }

        public async Task<Response> Update(Guid id, UpdateCategoryModel model)
        {
            var isExistedCode = await _unitOfWork.CategoryRepository.Query().AnyAsync(x => x.Name == model.Name);
            if (isExistedCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Hạng xe đã tồn tại!"
                };
            }
            var entity = await _unitOfWork.CategoryRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
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
            // entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.CategoryRepository.Update(entity);
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

            var checkExistedReferenceToVehicle = await _unitOfWork.PriceOfRentingServiceRepository
                                                                .Query()
                                                                .AnyAsync(x => x.CategoryId == id && x.Status == 1);
            if (checkExistedReferenceToVehicle)
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
