using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Vehicle;
using TourismSmartTransportation.Business.ViewModel.Admin.Vehicle;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;

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
                        .Select(item => item.AsVehicleTypeViewModel())
                        .ToListAsync();

            return list;
        }

        public async Task<Response> GetVehicleType(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.VehicleTypeRepository.GetById(id);
                if (entity == null)
                    return new Response(404, "Not found");

                var result = entity.AsVehicleTypeViewModel();

                return new Response(200, result);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<Response> CreateVehicleType(CreateVehicleModel model)
        {
            try
            {
                var entity = new VehicleType()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Seats = model.Seats,
                    Fuel = model.Fuel != null ? model.Fuel.Value : 0,
                    Status = model.Status != null ? model.Status.Value : 1
                };
                await _unitOfWork.VehicleTypeRepository.Add(entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response(201);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }

        }

        public async Task<Response> UpdateVehicleType(Guid id, CreateVehicleModel model)
        {
            try
            {
                var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
                if (entity is null)
                    return new Response(404, "Not Found");

                entity.Name = UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
                entity.Seats = UpdateTypeOfNotNullAbleObject<int>(entity.Seats, model.Seats);
                entity.Fuel = UpdateTypeOfNotNullAbleObject<int>(entity.Fuel, model.Fuel);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

                _unitOfWork.VehicleTypeRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response(204);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }

        public async Task<Response> DeleteVehicleType(Guid id)
        {
            try
            {
                var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
                if (entity is null)
                    return new Response(404, "Not found");

                entity.Status = 2;
                _unitOfWork.VehicleTypeRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new Response(200);
            }
            catch (Exception e)
            {
                return new Response(500, e.Message.ToString());
            }
        }
    }
}