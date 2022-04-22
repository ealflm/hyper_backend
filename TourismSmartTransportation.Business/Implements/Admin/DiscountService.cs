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

        public async Task<bool> CreateDiscount(CreateDiscountModel model)
        {
            var entity = new Discount()
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Code = model.Code,
                Description = model.Description,
                TimeStart = model.TimeStart.Value,
                TimeEnd = model.TimeEnd.Value,
                PhotoUrls = model.PhotoUrls,
                Value = model.Value.Value,
                Status = 1
            };
            await _unitOfWork.DiscountRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity is null)
                return false;

            entity.Status = 0;
            _unitOfWork.DiscountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;

        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
                return null;
            return entity.AsDiscountViewModel();
        }

        public async Task<SearchResultViewModel<DiscountViewModel>> SearchDiscount(DiscountSearchModel model)
        {
            Func<object, SearchResultViewModel<DiscountViewModel>> returnFunc = (param) =>
            {
                DiscountSearchModel model = (DiscountSearchModel)param;
                var source = _unitOfWork.DiscountRepository
                            .FindAsNoTracking()
                            .FilterFunc(model);
                var totalItems = source.Count();
                var items = source
                                .OrderByCustomFunc(model.SortBy)
                                .PaginateFunc(model.PageIndex, model.ItemsPerPage)
                                .Select(item => item.AsDiscountViewModel())
                                .ToList();
                var pageSize = GetPageSize(model.ItemsPerPage, totalItems);
                return new SearchResultViewModel<DiscountViewModel>(items, pageSize, totalItems);
            };

            Task<SearchResultViewModel<DiscountViewModel>> task = new Task<SearchResultViewModel<DiscountViewModel>>(returnFunc, model);
            task.Start();
            return await task;

        }

        public async Task<bool> UpdateDiscount(Guid id, CreateDiscountModel model)
        {

            var entity = _unitOfWork.DiscountRepository.GetById(id).Result;
            if (entity is null) return false;

            entity.Title = UpdateTypeOfNullAbleObject<string>(entity.Title, model.Title);
            entity.Code = UpdateTypeOfNullAbleObject<string>(entity.Code, model.Code);
            entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
            entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
            entity.Value = UpdateTypeOfNotNullAbleObject<decimal>(entity.Value, model.Value);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.DiscountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}