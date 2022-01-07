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

        public async Task<DataModel> CreateDiscount(CreateDiscountModel model)
        {
            try
            {
                var entity = new Discount()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Code = model.Code,
                    TimeStart = model.TimeStart,
                    TimeEnd = model.TimeEnd,
                    Value = model.Value,
                    Status = model.Status != null ? model.Status.Value : 1
                };

                await _unitOfWork.DiscountRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync();

                return new DataModel(201, entity, "");
            }
            catch (Exception)
            {
                return new DataModel(400, null, "");
            }
        }

        public async Task<DataModel> DeleteDiscount(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.DiscountRepository.GetById(id);
                if (entity is null)
                    return new DataModel(404, null, "Not found");

                entity.Status = 2;
                _unitOfWork.DiscountRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new DataModel(200, null, "");
            }
            catch (Exception e)
            {
                return new DataModel(400, null, e.Message.ToString());
            }
        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
                return null;

            return new DiscountViewModel()
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
            int totalRecord = _unitOfWork.DiscountRepository.Query().Count();
            if (totalRecord == 0)
                return null;

            var entity = await _unitOfWork.DiscountRepository
                            .Query()
                            .Where(item => model.Title == null || item.Title.Contains(model.Title))
                            .Where(item => model.Code == null || item.Code.Contains(model.Code))
                            .Where(item => model.Value == null || item.Value.ToString().Contains(model.Value.ToString()))
                            .Where(item => model.TimeStart == null || item.TimeStart.ToString().Contains(model.TimeStart.ToString()))
                            .Where(item => model.TimeEnd == null || item.TimeEnd.ToString().Contains(model.TimeEnd.ToString()))
                            .OrderBy(item => item.TimeStart)
                            .Skip(model.TotalItem * Math.Max(model.PageIndex - 1, 0))
                            .Take(model.TotalItem > 0 ? model.TotalItem : totalRecord)
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

            SearchDiscountResultViewModel searchResult = new()
            {
                Items = entity,
                PageSize = model.TotalItem == 0
                                        ? 1
                                        : (totalRecord / model.TotalItem) + (totalRecord % model.TotalItem > 0 ? 1 : 0)
            };

            return searchResult;
        }

        public async Task<DataModel> UpdateDiscount(Guid id, DiscountSearchModel model)
        {
            try
            {
                var entity = _unitOfWork.DiscountRepository.GetById(id).Result;
                if (entity is null)
                    return new DataModel(404, null, "Not Found");

                entity.Title = UpdateTypeOfNullAbleObject<string>(entity.Title, model.Title);
                entity.Code = UpdateTypeOfNullAbleObject<string>(entity.Code, model.Code);
                entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
                entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
                entity.Value = UpdateTypeOfNotNullAbleObject<decimal>(entity.Value, model.Value);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

                _unitOfWork.DiscountRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();

                return new DataModel(204, null, "");
            }
            catch (Exception e)
            {
                return new DataModel(500, null, e.Message.ToString());
            }
        }
    }
}