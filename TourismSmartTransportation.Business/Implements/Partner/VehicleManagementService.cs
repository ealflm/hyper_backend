using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class VehicleManagementService : BaseService, IVehicleManagementService
    {
        public VehicleManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> Create(CreateVehicleModel model)
        {
            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            var vehicle = new TourismSmartTransportation.Data.Models.Vehicle()
            {
                VehicleId = Guid.NewGuid(),
                ServiceTypeId = model.ServiceTypeId.Value,
                VehicleTypeId = model.VehicleTypeId.Value,
                RentStationId = model.RentStationId,
                PartnerId = model.PartnerId.Value,
                PriceRentingId = model.PriceRentingId.Value,
                Name = model.Name,
                LicensePlates = model.LicensePlates,
                Color = model.Color,
                Status = 1
            };
            await _unitOfWork.VehicleRepository.Add(vehicle);

            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới phương tiện thành công!"
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.VehicleRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            var result = await CheckReferenceToOther(id);
            if (result.StatusCode != 0)
            {
                return result;
            }

            entity.Status = 0;
            _unitOfWork.VehicleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<VehicleViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.VehicleRepository.GetById(id);
            var model = entity.AsVehicleViewModel();
            model.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(model.ServiceTypeId)).Name;
            model.CompanyName = (await _unitOfWork.PartnerRepository.GetById(model.PartnerId)).CompanyName;
            model.VehicleTypeName = (await _unitOfWork.VehicleTypeRepository.GetById(model.VehicleTypeId)).Label;
            return model;

        }

        public async Task<List<VehicleViewModel>> Search(VehicleSearchModel model)
        {
            var entity = await _unitOfWork.VehicleRepository.Query()
                            .Where(x => model.VehicleTypeId == null || x.VehicleTypeId.Equals(model.VehicleTypeId.Value))
                            .Where(x => model.ServiceTypeId == null || x.ServiceTypeId == model.ServiceTypeId.Value)
                            .Where(x => model.RentStationId == null || x.RentStationId == model.RentStationId.Value)
                            .Where(x => model.PriceRentingId == null || x.PriceRentingId == model.PriceRentingId.Value)
                            .Where(x => model.PartnerId == null || x.PartnerId == model.PartnerId.Value)
                            .Where(x => model.Color == null || x.Color.Contains(model.Color))
                            .Where(x => model.LicensePlates == null || x.LicensePlates.Contains(model.LicensePlates))
                            .Where(x => model.Name == null || x.Name.Contains(model.Name))
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsVehicleViewModel())
                            .ToListAsync();
            foreach (VehicleViewModel x in entity)
            {
                x.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(x.ServiceTypeId)).Name;
                x.CompanyName = (await _unitOfWork.PartnerRepository.GetById(x.PartnerId)).CompanyName;
                x.VehicleTypeName = (await _unitOfWork.VehicleTypeRepository.GetById(x.VehicleTypeId)).Label;
            }
            return entity;

        }

        public async Task<Response> Update(Guid id, UpdateVehicleModel model)
        {
            var entity = await _unitOfWork.VehicleRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            // Check status
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

            entity.RentStationId = UpdateTypeOfNotNullAbleObject<Guid>(entity.RentStationId, model.RentStationId);
            entity.ServiceTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.ServiceTypeId, model.ServiceTypeId);
            entity.VehicleTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleTypeId, model.VehicleTypeId);
            entity.PriceRentingId = UpdateTypeOfNotNullAbleObject<Guid>(entity.PriceRentingId, model.PriceRentingId);
            entity.Color = UpdateTypeOfNullAbleObject<string>(entity.Color, model.Color);
            entity.LicensePlates = UpdateTypeOfNullAbleObject<string>(entity.LicensePlates, model.LicensePlates);
            entity.Name = UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            entity.IsRunning = UpdateTypeOfNotNullAbleObject<int>(entity.IsRunning, model.IsRunning);
            _unitOfWork.VehicleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        private async Task<Response> CheckReferenceToOther(Guid id)
        {
            var checkExistedReferenceToTrip = await _unitOfWork.TripRepository
                                            .Query()
                                            .AnyAsync(
                                                            x => x.VehicleId == id &&
                                                            DateTime.Compare(DateTime.Now, x.TimeStart) >= 0 &&
                                                            DateTime.Compare(DateTime.Now, x.TimeEnd) <= 0
                                                    );
            if (checkExistedReferenceToTrip)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Dữ liệu đã được tham chiếu, bạn không thể xóa dữ liệu này"
                };
            }

            return new()
            {
                StatusCode = 0
            };
        }

        private async Task<Response> CheckValidationData(VehicleModel model)
        {
            // Check LicensePlates
            var isExistCode = await _unitOfWork.VehicleRepository.Query().AnyAsync(x => x.LicensePlates.Equals(model.LicensePlates));
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Phương tiện đã tồn tại!"
                };
            }

            // Check Partner
            if (model.PartnerId != null)
            {
                var partner = await _unitOfWork.PartnerRepository.GetById(model.PartnerId.Value);
                if (partner == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Đối tác không tồn tại"
                    };
                }
                else if (partner.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với đối tác đã bị vô hiệu hóa"
                    };
                }
            }

            // Check ServiceType
            if (model.ServiceTypeId != null)
            {
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(model.ServiceTypeId.Value);
                if (serviceType == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Loại dịch vụ không tồn tại"
                    };
                }
                else if (serviceType.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với loại dịch vụ đã bị vô hiệu hóa"
                    };
                }
            }

            // Check VehicleType
            if (model.VehicleTypeId != null)
            {
                var vehicleType = await _unitOfWork.VehicleTypeRepository.GetById(model.VehicleTypeId.Value);
                if (vehicleType == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Loại xe không tồn tại"
                    };
                }
                else if (vehicleType.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với loại xe đã bị vô hiệu hóa"
                    };
                }
            }

            // Check Rent station
            if (model.RentStationId != null)
            {
                var rentStation = await _unitOfWork.RentStationRepository.GetById(model.RentStationId.Value);
                if (rentStation == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Trạm thuê xe không tồn tại"
                    };
                }
                else if (rentStation.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với trạm thuê xe đã bị vô hiệu hóa"
                    };
                }
            }

            // Check price renting
            if (model.PriceRentingId != null)
            {
                var priceRenting = await _unitOfWork.PriceOfRentingServiceRepository.GetById(model.PriceRentingId.Value);
                if (priceRenting == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Giá thuê xe không tồn tại"
                    };
                }
                else if (priceRenting.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với giá thuê xe đã bị vô hiệu hóa"
                    };
                }
            }

            // Data is availiable
            return new()
            {
                StatusCode = 0, // No Error
            };
        }
    }
}