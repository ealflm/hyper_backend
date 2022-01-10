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

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class DiscountService : BaseService, IDiscountService
    {
        public DiscountService(IUnitOfWork unitOfWork) : base(unitOfWork)
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

            return new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Code = entity.Code,
                TimeStart = entity.TimeStart,
                TimeEnd = entity.TimeEnd,
                Value = entity.Value,
                Status = entity.Status
            };
        }

        public async Task<List<DiscountViewModel>> GetListDiscounts()
        {
            var list = await _unitOfWork.DiscountRepository
                        .Query()
                        .Select(item => new DiscountViewModel()
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Code = item.Code,
                            TimeStart = item.TimeStart,
                            TimeEnd = item.TimeEnd,
                            Value = item.Value,
                            Status = item.Status
                        })
                        .ToListAsync();

            return list;
        }

        public async Task<SearchDiscountResultViewModel> SearchDiscount(DiscountSearchModel model)
        {
            var listItemsAfterQuery = await _unitOfWork.DiscountRepository
                            .Query()
                            .Where(item => model.Title == null || item.Title.Contains(model.Title))
                            .Where(item => model.Code == null || item.Code.Contains(model.Code))
                            .Where(item => model.Value == null || item.Value.ToString().Contains(model.Value.Value.ToString()))
                            .Where(item => model.TimeStart == null || item.TimeStart >= model.TimeStart.Value)
                            .Where(item => model.TimeEnd == null || item.TimeEnd <= model.TimeEnd.Value.AddDays(1))
                            .Where(item => model.Status == null || item.Status == model.Status.Value)
                            .OrderBy(item => item.TimeStart)
                            .Select(item => new DiscountViewModel()
                            {
                                Id = item.Id,
                                Title = item.Title,
                                Code = item.Code,
                                TimeStart = item.TimeStart,
                                TimeEnd = item.TimeEnd,
                                Value = item.Value,
                                Status = item.Status
                            })
                            .ToListAsync();

            var totalRecord = listItemsAfterQuery.Count();
            if (totalRecord == 0 || GetPageSize(model.ItemsPerPage, totalRecord) < model.PageIndex)
                return null;

            var listItemsAfterPaging = listItemsAfterQuery.AsQueryable()
                            // .Skip(model.ItemsPerPage < totalRecord ? model.ItemsPerPage * Math.Max(model.PageIndex - 1, 0) : 0)
                            // .Take(model.ItemsPerPage < totalRecord && model.ItemsPerPage > 0 ? model.ItemsPerPage : totalRecord)
                            .Skip(SkipItemsOfPagingFunc(model.ItemsPerPage, totalRecord, model.PageIndex))
                            .Take(TakeItemsOfPagingFunc(model.ItemsPerPage, totalRecord))
                            .ToList();

            SearchDiscountResultViewModel searchResult = new()
            {
                Items = listItemsAfterPaging,
                TotalItems = totalRecord,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord)
            };

            return searchResult;
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