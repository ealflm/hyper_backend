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

        public async Task<Response> Add(PublishYearSearchModel model)
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
                Id = Guid.NewGuid(),
                Name= model.Name,
                Description= model.Description,
                Status = 1
            };
            await _unitOfWork.PublishYearRepository.Add(entity);
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
                .Where(x => model.Name == null || model.Name.Equals(model.Name))
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

        public async Task<Response> Update(Guid id, PublishYearSearchModel model)
        {
            var isExistedCode = await _unitOfWork.CategoryRepository.Query().AnyAsync(x => x.Name == model.Name);
            if (isExistedCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Năm sản xuất đã tồn tại!"
                };
            }
            var entity = await _unitOfWork.PublishYearRepository.GetById(id);
            if (entity is null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }
            
            entity.Name = UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
            entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
            entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.PublishYearRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}
