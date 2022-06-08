using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class VehicleTypeService : BaseService, IVehicleTypeService
    {
        public VehicleTypeService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
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

        public async Task<VehicleTypeViewModel> GetVehicleType(Guid id)
        {
            var entity = await _unitOfWork.VehicleTypeRepository.GetById(id);
            if (entity == null)
            {
                return null;
            }
            return entity.AsVehicleTypeViewModel();
        }

        public async Task<bool> CreateVehicleType(CreateVehicleTypeModel model)
        {
            var entity = new VehicleType()
            {
                Id = Guid.NewGuid(),
                Label = model.Label,
                Seats = model.Seats,
                Fuel = model.Fuel,
                Status = model.Status != null ? model.Status.Value : 1
            };
            await _unitOfWork.VehicleTypeRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateVehicleType(Guid id, CreateVehicleTypeModel model)
        {
            try
            {
                var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
                if (entity is null)
                {
                    return false;
                }

                entity.Label = UpdateTypeOfNullAbleObject<string>(entity.Label, model.Label);
                entity.Seats = UpdateTypeOfNotNullAbleObject<int>(entity.Seats, model.Seats);
                entity.Fuel = UpdateTypeOfNullAbleObject<string>(entity.Fuel, model.Fuel);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

                _unitOfWork.VehicleTypeRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteVehicleType(Guid id)
        {
            try
            {
                var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
                if (entity is null)
                {
                    return false;
                }

                entity.Status = 2;
                _unitOfWork.VehicleTypeRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}