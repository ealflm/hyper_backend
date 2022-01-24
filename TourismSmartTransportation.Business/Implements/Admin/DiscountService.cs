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
            try
            {
                var entity = new Discount()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Code = model.Code,
                    TimeStart = model.TimeStart.Value,
                    TimeEnd = model.TimeEnd.Value,
                    Value = model.Value.Value,
                    Status = model.Status != null ? model.Status.Value : 1
                };

                await _unitOfWork.DiscountRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync();

                return new Response(201);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<Response> DeleteDiscount(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.DiscountRepository.GetById(id);
                if (entity is null)
                    return new Response(404, "Not found");

                entity.Status = 2;
                _unitOfWork.DiscountRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response(200);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
                return null;

            return entity.AsDiscountViewModel();
        }

        public async Task<List<DiscountViewModel>> GetListDiscounts()
        {
            var list = await _unitOfWork.DiscountRepository
                        .Query()
                        .Select(item => item.AsDiscountViewModel())
                        .ToListAsync();

            return list;
        }

        public SearchResultViewModel<DiscountViewModel> SearchDiscount(DiscountSearchModel model)
        {
            var source = _unitOfWork.DiscountRepository
                            .FindAsNoTracking()
                            .FilterFunc(model);

            var totalItems = source.Count();
            var items = source.AsQueryable()
                            .OrderByCustomFunc(model.SortBy)
                            .PaginateFunc(model.PageIndex, model.ItemsPerPage)
                            .Select(item => item.AsDiscountViewModel())
                            .ToList();
            var pageSize = GetPageSize(model.ItemsPerPage, totalItems);

            return new SearchResultViewModel<DiscountViewModel>(items, pageSize, totalItems);
        }

        public async Task<Response> UpdateDiscount(Guid id, CreateDiscountModel model)
        {
            try
            {
                var entity = _unitOfWork.DiscountRepository.GetById(id).Result;
                if (entity is null)
                    return new Response(404, "Not Found");

                entity.Title = UpdateTypeOfNullAbleObject<string>(entity.Title, model.Title);
                entity.Code = UpdateTypeOfNullAbleObject<string>(entity.Code, model.Code);
                entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
                entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
                entity.Value = UpdateTypeOfNotNullAbleObject<decimal>(entity.Value, model.Value);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

                _unitOfWork.DiscountRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                return new Response(204);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }
    }
}