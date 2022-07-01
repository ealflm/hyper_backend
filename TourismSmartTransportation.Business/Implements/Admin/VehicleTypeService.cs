using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class VehicleTypeService : BaseService, IVehicleTypeService
    {
        public VehicleTypeService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<List<VehicleTypeViewModel>> GetListVehicleTypes(VehicleTypeSearchModel model)
        {
            var list = await _unitOfWork.VehicleTypeRepository
                        .Query()
                        .Where(x => model.Label == null || x.Label.Contains(model.Label))
                        .Where(x => model.Fuel == null || x.Fuel.Contains(model.Fuel))
                        .Where(x => model.Seats == null || x.Seats.ToString().Contains(model.Seats.ToString()))
                        .Where(x => model.Status == null || x.Status == model.Status.Value)
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

        public async Task<Response> CreateVehicleType(CreateVehicleTypeModel model)
        {
            var entity = new VehicleType()
            {
                VehicleTypeId = Guid.NewGuid(),
                Label = model.Label,
                Seats = model.Seats,
                Fuel = model.Fuel,
                Status = 1
            };
            await _unitOfWork.VehicleTypeRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới loại xe thành công!"
            };

        }

        public async Task<Response> UpdateVehicleType(Guid id, UpdateVehicleTypeModel model)
        {
            var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }

            if (model.Status == null)
            {
                entity.Status = entity.Status;
            }
            else if (model.Status.Value == 0)
            {
                var result = await CheckReferenceToOther(id);
                if (result.StatusCode != 0)
                {
                    return result;
                }
                entity.Status = 0;
            }
            else
            {
                entity.Status = model.Status.Value;
            }

            entity.Label = UpdateTypeOfNullAbleObject<string>(entity.Label, model.Label);
            entity.Seats = UpdateTypeOfNotNullAbleObject<int>(entity.Seats, model.Seats);
            entity.Fuel = UpdateTypeOfNullAbleObject<string>(entity.Fuel, model.Fuel);

            _unitOfWork.VehicleTypeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật loại xe thành công!"
            };
        }

        public async Task<Response> DeleteVehicleType(Guid id)
        {
            var entity = _unitOfWork.VehicleTypeRepository.GetById(id).Result;
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }

            var result = await CheckReferenceToOther(id);
            if (result.StatusCode != 0)
            {
                return result;
            }

            entity.Status = 0;
            _unitOfWork.VehicleTypeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        private async Task<Response> CheckReferenceToOther(Guid id)
        {
            var obj = new Response()
            {
                StatusCode = 400,
                Message = "Dữ liệu đã được tham chiếu, bạn không thể xóa dữ liệu này"
            };

            var checkExistedReferenceToVehicle = await _unitOfWork.VehicleRepository
                                                                .Query()
                                                                .AnyAsync(x => x.VehicleId == id && x.Status == 1);
            if (checkExistedReferenceToVehicle)
            {
                return obj;
            }

            var checkExistedReferenceToPriceOfBookingService = await _unitOfWork.PriceOfBookingServiceRepository
                                                                    .Query()
                                                                    .AnyAsync(x => x.VehicleTypeId == id && x.Status == 1);
            if (checkExistedReferenceToPriceOfBookingService)
            {
                return obj;
            }

            return new()
            {
                StatusCode = 0
            };
        }
    }
}