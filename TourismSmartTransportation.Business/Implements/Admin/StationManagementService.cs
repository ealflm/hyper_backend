using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using Azure.Storage.Blobs;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class StationManagementService : BaseService, IStationManagementService
    {
        public StationManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<bool> AddStation(AddStationViewModel model)
        {
            try
            {
                var station = new Station()
                {
                    Address = model.Address,
                    Name = model.Name,
                    Latitude = model.Latitude.Value,
                    Longitude = model.Longitude.Value,
                    Status = 1
                };
                await _unitOfWork.StationRepository.Add(station);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteStation(Guid id)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetById(id);
                station.Status = 2;
                _unitOfWork.StationRepository.Update(station);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<StationViewModel> GetStation(Guid id)
        {
            var station = await _unitOfWork.StationRepository.GetById(id);
            if (station == null)
            {
                return null;
            }
            StationViewModel model = station.AsStationViewModel();
            return model;
        }

        public async Task<SearchResultViewModel<StationViewModel>> SearchStation(StationSearchModel model)
        {
            var stations = await _unitOfWork.StationRepository.Query()
                .Where(x => model.Name == null || x.Name.Contains(model.Name))
                .Where(x => model.Address == null || x.Address.Contains(model.Address))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Name)
                .Select(x => x.AsStationViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(stations, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<StationViewModel> result = null;
                result = new SearchResultViewModel<StationViewModel>()
                {
                    Items = listItemsAfterPaging,
                    PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                    TotalItems = totalRecord
                };
            return result;
        }

        public async Task<bool> UpdateStation(Guid id, AddStationViewModel model)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetById(id);
                station.Name = UpdateTypeOfNullAbleObject<string>(station.Name, model.Name);
                station.Address = UpdateTypeOfNullAbleObject<string>(station.Address, model.Address);
                station.Latitude = UpdateTypeOfNotNullAbleObject<decimal>(station.Latitude, model.Latitude);
                station.Longitude = UpdateTypeOfNotNullAbleObject<decimal>(station.Longitude, model.Longitude);
                station.Status = UpdateTypeOfNotNullAbleObject<int>(station.Status, model.Status);
                _unitOfWork.StationRepository.Update(station);
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
