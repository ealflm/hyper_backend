using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Vehicle;
using TourismSmartTransportation.Business.ViewModel.Admin.Vehicle;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class VehicleTypeService : BaseService, IVehicleTypeService
    {
        public VehicleTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<List<VehicleTypeViewModel>> GetListVehicleTypes()
        {
            var list = await _unitOfWork.VehicleTypeRepository.Query()
                        .Select(item => new VehicleTypeViewModel()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Seats = item.Seats,
                            Fuel = item.Fuel,
                            Status = item.Status
                        })
                        .ToListAsync();

            return list;
        }

        public async Task<VehicleTypeViewModel> GetVehicleType(Guid id)
        {
            var result = await _unitOfWork.VehicleTypeRepository.Query()
                            .Where(item => item.Id == id)
                            .Select(item => new VehicleTypeViewModel()
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Fuel = item.Fuel,
                                Seats = item.Seats,
                                Status = item.Status
                            })
                            .SingleOrDefaultAsync();

            return result;
        }

        public async Task<VehicleTypeViewModel> CreateVehicleType(VehicleTypeSearchModel model)
        {
            var result = _unitOfWork.VehicleTypeRepository.Query()
                            .Add(new VehicleType()
                            {
                                Id = Guid.NewGuid(),
                                Name = model.Name,
                                Seats = model.Seats,
                                Fuel = model.Fuel,
                                Status = model.Status
                            });
            await _unitOfWork.SaveChangesAsync();

            return new VehicleTypeViewModel()
            {
                Id = result.Entity.Id,
                Name = result.Entity.Name,
                Seats = result.Entity.Seats,
                Fuel = result.Entity.Fuel,
                Status = result.Entity.Status
            };
        }

        public async Task<bool> UpdateVehicleType(Guid id, VehicleTypeSearchModel model)
        {
            var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
            if (entity is null)
                return false;

            entity.Name = model.Name;
            entity.Seats = model.Seats;
            entity.Fuel = model.Fuel;
            entity.Status = model.Status;
            _unitOfWork.VehicleTypeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVehicleType(Guid id)
        {
            var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
            if (entity is null)
                return false;

            entity.Status = 0;
            _unitOfWork.VehicleTypeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}