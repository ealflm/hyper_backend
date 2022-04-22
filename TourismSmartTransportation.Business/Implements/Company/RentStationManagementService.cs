using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Business.SearchModel.Company.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Company.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;

namespace TourismSmartTransportation.Business.Implements.Company
{
    public class RentStationManagementService : BaseService, IRentStationManagementService
    {
        public RentStationManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<bool> AddRentStation(AddRentStationViewModel model)
        {

            var rentStation = new RentStation()
            {
                Name = model.Name,
                Latitude = model.Latitude.Value,
                Longitude = model.Longitude.Value,
                Status = 1
            };
            await _unitOfWork.RentStationRepository.Add(rentStation);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRentStation(Guid id)
        {

            var rentStation = await _unitOfWork.RentStationRepository.GetById(id);
            if (rentStation == null) return false;
            rentStation.Status = 1;
            _unitOfWork.RentStationRepository.Update(rentStation);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<RentStationViewModel> GetRentStation(Guid id)
        {
            var rentStation = await _unitOfWork.RentStationRepository.GetById(id);
            if (rentStation == null)
            {
                return null;
            }
            RentStationViewModel model = rentStation.AsRentStationViewModel();
            return model;
        }

        public async Task<SearchResultViewModel<RentStationViewModel>> SearchRentStation(RentStationSearchModel model)
        {
            Func<object, SearchResultViewModel<RentStationViewModel>> returnFunc = (param) =>
            {
                RentStationSearchModel model = (RentStationSearchModel)param;
                var source = _unitOfWork.RentStationRepository
                            .FindAsNoTracking()
                            .FilterFunc(model);
                var totalItems = source.Count();
                var items = source
                                .OrderByCustomFunc(model.SortBy)
                                .PaginateFunc(model.PageIndex, model.ItemsPerPage)
                                .Select(item => item.AsRentStationViewModel())
                                .ToList();
                var pageSize = GetPageSize(model.ItemsPerPage, totalItems);
                return new SearchResultViewModel<RentStationViewModel>(items, pageSize, totalItems);
            };

            Task<SearchResultViewModel<RentStationViewModel>> task = new Task<SearchResultViewModel<RentStationViewModel>>(returnFunc, model);
            task.Start();
            return await task;
        }

        public async Task<bool> UpdateRentStaion(Guid id, AddRentStationViewModel model)
        {
            try
            {
                var rentStation = await _unitOfWork.RentStationRepository.GetById(id);
                rentStation.Name = UpdateTypeOfNullAbleObject<string>(rentStation.Name, model.Name);
                rentStation.Latitude = UpdateTypeOfNotNullAbleObject<decimal>(rentStation.Latitude, model.Latitude);
                rentStation.Longitude = UpdateTypeOfNotNullAbleObject<decimal>(rentStation.Longitude, model.Longitude);
                rentStation.Status = UpdateTypeOfNotNullAbleObject<int>(rentStation.Status, model.Status);
                _unitOfWork.RentStationRepository.Update(rentStation);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
