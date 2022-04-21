using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.Service;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class ServiceManagement : BaseService, IServiceManagementService
    {
        public ServiceManagement(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
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
                    TimeStart = model.TimeStart.Value,
                    TimeEnd = model.TimeEnd.Value,
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

                var result = entity.AsServiceViewModel();

                return new Response(200, result);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }
        public async Task<SearchServiceResultViewModel> SearchServices(ServiceSearchModel model)
        {
            var listAfterSearching = await _unitOfWork.ServiceRepository
                        .Query()
                        .Where(item => model.Title == null || item.Title.Contains(model.Title))
                        .Where(item => model.Description == null || item.Description.Contains(model.Description))
                        .Where(item => model.Price == null || item.Price == model.Price.Value)
                        .Where(item => model.TimeStart == null || model.TimeStart.Value <= item.TimeStart && item.TimeStart < model.TimeStart.Value.AddDays(1))
                        .Where(item => model.TimeEnd == null || model.TimeEnd.Value <= item.TimeEnd && item.TimeEnd < model.TimeEnd.Value.AddDays(1))
                        .Where(item => model.Status == null || item.Status == model.Status.Value)
                        // .OrderByDynamicProperty(SortBy(model.SortBy, "Time"))
                        .Select(item => item.AsServiceViewModel())
                        .ToListAsync();

            var listAfterQuery = GetListAfterSorting(listAfterSearching, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSearching, model.ItemsPerPage, model.PageIndex);
            if (totalRecord == 0)
                return null;

            var listItemsAfterPaging = GetListAfterPaging(listAfterQuery, model.ItemsPerPage, model.PageIndex, totalRecord);

            SearchServiceResultViewModel searchResult = new()
            {
                // Items = listItemsAfterPaging,
                // TotalItems = totalRecord,
                // PageSize = GetPageSize(model.ItemsPerPage, totalRecord)
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
                entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
                entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
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