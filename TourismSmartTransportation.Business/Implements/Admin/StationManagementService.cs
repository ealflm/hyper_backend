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
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class StationManagementService : BaseService, IStationManagementService
    {
        public StationManagementService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
            StationViewModel model = new StationViewModel()
            {
                Id = station.Id,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Name = station.Name,
                Status = station.Status
            };
            return model;
        }

        public async Task<SearchResultViewModel> SearchStation(StationSearchModel model)
        {
            int stationCount = _unitOfWork.StationRepository.Query().Count();
            var stations = await _unitOfWork.StationRepository.Query()
                .Where(x => model.Name == null || x.Name.Contains(model.Name))
                .Where(x => model.Address == null || x.Address.Contains(model.Address))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Name)
                .Skip(model.ItemsPerPage * Math.Min(model.PageIndex - 1, 0))
                .Take(model.ItemsPerPage > 0 ? model.ItemsPerPage : stationCount)
                .Select(x => new StationViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Status = x.Status
                })
                .ToListAsync();
            SearchResultViewModel result = null;
            if (stations.Count > 0)
            {
                result = new SearchResultViewModel()
                {
                    Items = stations.ToList<object>(),
                    PageSize = model.ItemsPerPage == 0 ? 1 : ((stationCount / model.ItemsPerPage) + (stationCount % model.ItemsPerPage > 0 ? 1 : 0))
                };
            }
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
