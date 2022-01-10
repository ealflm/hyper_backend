using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.Service;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class ServiceManagement : BaseService, IServiceManagementService
    {
        public ServiceManagement(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Response> CreateService(CreateServiceModel model)
        {
            try
            {
                var entity = new Service()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price.Value,
                    Time = model.Time.Value,
                    PhotoUrls = model.PhotoUrls != null ? model.PhotoUrls : "",
                    Status = model.Status.Value
                };

                await _unitOfWork.ServiceRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response(201);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<Response> DeleteService(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.ServiceRepository.GetById(id);
                if (entity is null)
                    return new Response(404, "Not found");

                entity.Status = 2;

                _unitOfWork.ServiceRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response(200);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<Response> GetService(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.ServiceRepository.GetById(id);
                if (entity is null)
                    return new Response(404, "Not found");

                var result = new ServiceViewModel()
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    Price = entity.Price,
                    Time = entity.Time,
                    PhotoUrls = entity.PhotoUrls,
                    Status = entity.Status
                };

                return new Response(200, result);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<SearchServiceResultViewModel> SearchServices(ServiceSearchModel model)
        {
            var listItemsAfterQuery = await _unitOfWork.ServiceRepository
                        .Query()
                        .Where(item => model.Title == null || item.Title.Contains(model.Title))
                        .Where(item => model.Description == null || item.Description.Contains(model.Description))
                        .Where(item => model.Price == null || item.Price == model.Price.Value)
                        .Where(item => model.Time == null || model.Time.Value <= item.Time && item.Time < model.Time.Value.AddDays(1))
                        .Where(item => model.Status == null || item.Status == model.Status.Value)
                        .OrderByDescending(item => item.Time)
                        .Select(item => new ServiceViewModel()
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Description = item.Description,
                            Price = item.Price,
                            Time = item.Time,
                            PhotoUrls = item.PhotoUrls,
                            Status = item.Status
                        })
                        .ToListAsync();

            var totalRecord = listItemsAfterQuery.Count();
            if (totalRecord == 0 || GetPageSize(model.ItemsPerPage, totalRecord) < model.PageIndex)
                return null;

            var listItemsAfterPaging = listItemsAfterQuery.AsQueryable()
                                        .Skip(SkipItemsOfPagingFunc(model.ItemsPerPage, totalRecord, model.PageIndex))
                                        .Take(TakeItemsOfPagingFunc(model.ItemsPerPage, totalRecord))
                                        .ToList();

            SearchServiceResultViewModel searchResult = new()
            {
                Items = listItemsAfterPaging,
                TotalItems = totalRecord,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord)
            };

            return searchResult;
        }

        public async Task<Response> UpdateService(Guid id, CreateServiceModel model)
        {
            try
            {
                var entity = await _unitOfWork.ServiceRepository.GetById(id);
                if (entity is null)
                    return new Response(404, "Not found");

                entity.Title = UpdateTypeOfNullAbleObject<string>(entity.Title, model.Title);
                entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
                entity.Price = UpdateTypeOfNotNullAbleObject<decimal>(entity.Price, model.Price);
                entity.Time = UpdateTypeOfNotNullAbleObject<DateTime>(entity.Time, model.Time);
                entity.PhotoUrls = UpdateTypeOfNullAbleObject<string>(entity.PhotoUrls, model.PhotoUrls);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

                _unitOfWork.ServiceRepository.Update(entity);
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