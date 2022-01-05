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
            var list = await _unitOfWork.VehicleTypeRepository
                        .Query()
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
            var result = await _unitOfWork.VehicleTypeRepository
                            .Query()
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

        public async Task<bool> CreateVehicleType(VehicleTypeSearchModel model)
        {
            try
            {
                if (model.Name == null)
                    return false;

                var entity = new VehicleType()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Seats = model.Seats != null ? model.Seats.Value : 0,
                    Fuel = model.Fuel != null ? model.Fuel.Value : 0,
                    Status = model.Status != null ? model.Status.Value : 0
                };
                await _unitOfWork.VehicleTypeRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> UpdateVehicleType(Guid id, VehicleTypeSearchModel model)
        {
            var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
            if (entity is null)
                return false;

            entity.Name = model.Name != null && model.Name != entity.Name ? model.Name : entity.Name;
            entity.Seats = model.Seats != null && model.Seats != entity.Seats ? model.Seats.Value : entity.Seats;
            entity.Fuel = model.Fuel != null && model.Fuel != entity.Fuel ? model.Fuel.Value : entity.Fuel;
            entity.Status = model.Status != null && model.Status != entity.Status ? model.Status.Value : entity.Status;

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