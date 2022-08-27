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
using TourismSmartTransportation.Business.Hubs;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class VehicleManagementService : BaseService, IVehicleManagementService
    {
        private INotificationHub _notificationHub;
        public VehicleManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, INotificationHub notificationHub) : base(unitOfWork, blobServiceClient)
        {
            _notificationHub = notificationHub;
        }

        public async Task<Response> Create(CreateVehicleModel model)
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

            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            var priceRenting = await _unitOfWork.PriceOfRentingServiceRepository
                                    .Query()
                                    .Where(x => model.CategoryId == null ? false : x.CategoryId == model.CategoryId.Value &&
                                                model.PublishYearId == null ? false : x.PublishYearId == model.PublishYearId.Value)
                                    .FirstOrDefaultAsync();

            var vehicle = new TourismSmartTransportation.Data.Models.Vehicle()
            {
                VehicleId = Guid.NewGuid(),
                ServiceTypeId = model.ServiceTypeId.Value,
                VehicleTypeId = model.VehicleTypeId.Value,
                RentStationId = model.RentStationId,
                PartnerId = model.PartnerId.Value,
                PriceRentingId = priceRenting != null ? priceRenting.PriceOfRentingServiceId : null,
                Name = model.Name,
                LicensePlates = model.LicensePlates,
                Color = model.Color,
                Status = (int)VehicleStatus.Ready
            };
            await _unitOfWork.VehicleRepository.Add(vehicle);

            await _unitOfWork.SaveChangesAsync();

            // cập nhật lại danh sách driver cho notification hub
            await _notificationHub.LoadVehiclesList();

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

            entity.Status = (int)VehicleStatus.Disabled;
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
            if (entity.PriceRentingId != null)
            {
                var priceEntity = await _unitOfWork.PriceOfRentingServiceRepository.GetById(entity.PriceRentingId.Value);
                model.CategoryId = priceEntity.CategoryId;
                model.PublishYearId = priceEntity.PublishYearId;
            }
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
                var customerTrip = await _unitOfWork.CustomerTripRepository.Query().Where(y => y.VehicleId.Equals(x.Id)).ToListAsync();
                decimal rate = 0;
                int count = 0;
                foreach (CustomerTrip y in customerTrip)
                {
                    var feedback = await _unitOfWork.FeedbackForVehicleRepository.Query().Where(x => x.CustomerTripId.Equals(y.CustomerTripId)).FirstOrDefaultAsync();
                    if (feedback != null)
                    {
                        rate += feedback.Rate;
                        count++;
                    }

                }
                if (count > 0)
                {
                    x.Rate = rate / count;
                }
                if (x.PriceRentingId != null)
                {
                    var priceEntity = await _unitOfWork.PriceOfRentingServiceRepository.GetById(x.PriceRentingId.Value);
                    x.CategoryId = priceEntity.CategoryId;
                    x.PublishYearId = priceEntity.PublishYearId;
                }
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

            // if license plates does not edit, we still continute to update the other fileds
            // else we need to check other license plates
            if (entity.LicensePlates != model.LicensePlates && model.LicensePlates != null)
            {
                var isExistCode = await _unitOfWork.VehicleRepository.Query().AnyAsync(x => x.LicensePlates.Equals(model.LicensePlates));
                if (isExistCode)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Phương tiện đã tồn tại!"
                    };
                }
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
                // var result = await CheckReferenceToOther(id);
                // if (result.StatusCode != 0)
                // {
                //     return result;
                // }
                entity.Status = 0;
            }
            else
            {
                entity.Status = model.Status.Value;
            }

            if (model.CategoryId != null)
            {
                entity.PriceRentingId = (await _unitOfWork.PriceOfRentingServiceRepository
                .Query()
                .Where(x => x.CategoryId == model.CategoryId.Value && x.PublishYearId == model.PublishYearId.Value)
                .FirstOrDefaultAsync()).PriceOfRentingServiceId;
            }
            else
            {
                entity.PriceRentingId = entity.PriceRentingId;
            }

            entity.RentStationId = UpdateTypeOfNotNullAbleObject<Guid>(entity.RentStationId, model.RentStationId);
            entity.ServiceTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.ServiceTypeId, model.ServiceTypeId);
            entity.VehicleTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleTypeId, model.VehicleTypeId);
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
                                                            x => x.VehicleId == id
                                                    // &&
                                                    // DateTime.Compare(DateTime.Now, x.TimeStart) >= 0 &&
                                                    // DateTime.Compare(DateTime.Now, x.TimeEnd) <= 0
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
            if (model.CategoryId != null)
            {
                var priceRenting = await _unitOfWork.PriceOfRentingServiceRepository
                                    .Query()
                                    .Where(x => x.CategoryId == model.CategoryId.Value && x.PublishYearId == model.PublishYearId.Value)
                                    .FirstOrDefaultAsync();
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