using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class StationManagementService : BaseService,IStationManagementService
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
            StationViewModel model = new StationViewModel()
            {
                Id = station.Id,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Name = station.Name,
                Status = station.Status
            };
            return  model;
        }

        public async Task<SearchStationResultViewModel> SearchStation(StationSearchModel model = null)
        {
            if (model == null)
            {
                model = new StationSearchModel();
            }
            int stationCount = _unitOfWork.StationRepository.Query().Count();
            var stations = await _unitOfWork.StationRepository.Query()
                .Where(x => model.Name == null | x.Name.Contains(model.Name))
                .Where(x => model.Address == null | x.Address.Contains(model.Address))
                .Where(x => model.Status == 0 | x.Status == model.Status)
                .OrderBy(x => x.Name)
                .Skip(model.TotalItem * Math.Min(model.PageIndex - 1, 0))
                .Take(model.TotalItem > 0 ? model.TotalItem : stationCount)
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
            SearchStationResultViewModel result = null;
            if (stations.Count>0)
            {
                result = new SearchStationResultViewModel()
                {
                    Items = stations,
                    PageSize = model.TotalItem == 0 ? 1 : ((stationCount / model.TotalItem) + (stationCount % model.TotalItem > 0 ? 1 : 0))
                };
            }
            return result;
        }

        public async Task<bool> UpdateStation(Guid id, AddStationViewModel model)
        {
            try
            {
                var station = await _unitOfWork.StationRepository.GetById(id);
                station.Name = (model.Name != null) ? model.Name : station.Name;
                station.Address = (model.Address != null) ? model.Address : station.Address;
                station.Latitude = (model.Latitude !=null) ? model.Latitude.Value : station.Latitude;
                station.Longitude = (model.Longitude !=null) ? model.Longitude.Value : station.Longitude;
                station.Status = (model.Status != null) ? model.Status.Value : station.Status;
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
