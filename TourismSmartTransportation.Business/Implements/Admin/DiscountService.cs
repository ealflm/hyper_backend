using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class DiscountService : BaseService, IDiscountService
    {
        public DiscountService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreateDiscount(CreateDiscountModel model)
        {
            var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(model.ServiceTypeId);
            if (serviceType == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Loại dịch vụ không tồn tại!"
                };
            }

            var entity = new Discount()
            {
                DiscountId = Guid.NewGuid(),
                ServiceTypeId = model.ServiceTypeId,
                Title = model.Title,
                Description = model.Description,
                TimeStart = model.TimeStart,
                TimeEnd = model.TimeEnd,
                PhotoUrl = await UploadFile(model.UploadFile, Container.Admin),
                Value = model.Value,
                Status = 1
            };
            await _unitOfWork.DiscountRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới khuyến mãi thành công!"
            };
        }

        public async Task<Response> DeleteDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            entity.Status = 0;
            _unitOfWork.DiscountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };

        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
            {
                return null;
            }
            return entity.AsDiscountViewModel();
        }

        // public async Task<SearchResultViewModel<DiscountViewModel>> SearchDiscount(DiscountSearchModel model)
        // {
        //     Func<object, SearchResultViewModel<DiscountViewModel>> returnFunc = (param) =>
        //     {
        //         DiscountSearchModel model = (DiscountSearchModel)param;
        //         var source = _unitOfWork.DiscountRepository
        //                     .FindAsNoTracking()
        //                     .FilterFunc(model);
        //         var totalItems = source.Count();
        //         var items = source
        //                         .OrderByCustomFunc(model.SortBy)
        //                         .PaginateFunc(model.PageIndex, model.ItemsPerPage)
        //                         .Select(item => item.AsDiscountViewModel())
        //                         .ToList();
        //         var pageSize = GetPageSize(model.ItemsPerPage, totalItems);
        //         return new SearchResultViewModel<DiscountViewModel>(items, pageSize, totalItems);
        //     };

        //     Task<SearchResultViewModel<DiscountViewModel>> task = new Task<SearchResultViewModel<DiscountViewModel>>(returnFunc, model);
        //     task.Start();
        //     return await task;

        // }

        public async Task<SearchResultViewModel<DiscountViewModel>> SearchDiscount(DiscountSearchModel model)
        {
            var discount = await _unitOfWork.DiscountRepository.Query()
                .Where(x => model.Title == null || x.Title.Contains(model.Title))
                .Where(x => model.TimeStart == null || DateTime.Compare(x.TimeStart, model.TimeStart.Value) >= 0)
                .Where(x => model.TimeEnd == null || DateTime.Compare(x.TimeEnd, model.TimeEnd.Value) <= 0)
                .Where(x => model.Value == null || x.Value == model.Value.Value)
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Value)
                .Select(x => x.AsDiscountViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(discount, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<DiscountViewModel> result = null;
            result = new SearchResultViewModel<DiscountViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdateDiscount(Guid id, UpdateDiscountModel model)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }
            DateTime timeEnd = model.TimeEnd != null ? model.TimeEnd.Value : entity.TimeEnd;
            if (model.TimeStart != null && DateTime.Compare(model.TimeStart.Value, timeEnd) > 0)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = $"Thời gian bắt đầu không được lớn hơn thời gian kết thúc({timeEnd})"
                };
            }
            DateTime timeStart = model.TimeStart != null ? model.TimeStart.Value : entity.TimeStart;
            if (model.TimeEnd != null && DateTime.Compare(model.TimeEnd.Value, timeStart) < 0)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = $"Thời gian kết thúc không được nhỏ hơn thời gian bắt đầu({timeStart})"
                };
            }

            entity.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Admin, entity.PhotoUrl);
            entity.PhotoUrl += await UploadFile(model.UploadFile, Container.Admin);
            entity.ServiceTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.ServiceTypeId, model.ServiceTypeId);
            entity.Title = UpdateTypeOfNullAbleObject<string>(entity.Title, model.Title);
            entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
            entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
            entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
            entity.Value = UpdateTypeOfNotNullAbleObject<decimal>(entity.Value, model.Value);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.DiscountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}