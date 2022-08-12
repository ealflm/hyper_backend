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
using TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using Vonage.Request;
using System.Net.Http;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class DriverManagementService : AccountService, IDriverManagementService
    {
        // private readonly string MESSAGE = "Dang nhap bang SDT da dang ky voi MAT KHAU: ";

        public DriverManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, Credentials credentials, HttpClient client, ITwilioSettings twilioSettings) : base(unitOfWork, blobServiceClient, credentials, client, twilioSettings)
        {
        }

        public async Task<Response> Create(CreateDriverModel model)
        {
            var isExistCode = await _unitOfWork.DriverRepository.Query().AnyAsync(x => x.Phone.Equals(model.Phone));
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Tài xế đã tồn tại!"
                };
            }
            var random = new Random(DateTime.Now.Second);
            string password = random.Next(0, 10).ToString() + random.Next(0, 10).ToString() + random.Next(0, 10).ToString() + random.Next(0, 10).ToString();
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var driver = new Driver()
            {
                DriverId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                DateOfBirth = model.DateOfBirth,
                FirstName = model.FirstName,
                Gender = model.Gender,
                LastName = model.LastName,
                ModifiedDate = DateTime.Now,
                PartnerId = model.PartnerId,
                Phone = model.Phone,
                VehicleId = model.VehicleId != null ? model.VehicleId.Value : null,
                Password = passwordHash,
                Salt = passwordSalt,
                Status = 1
            };

            await _unitOfWork.DriverRepository.Add(driver);
            await _unitOfWork.SaveChangesAsync();
            // SendSMS(driver.Phone, MESSAGE + password);
            await SendSMSByTwilio(driver.Phone, $"Thông tin đăng nhập: \n Số điện thoại: {driver.Phone}\n Mã pin: {password}");
            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới tài xế thành công!"
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.DriverRepository.GetById(id);
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
            _unitOfWork.DriverRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<DriverViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.DriverRepository.GetById(id);
            var model = entity.AsDriverViewModel();
            if (model.VehicleId != null)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(model.VehicleId.Value);
                model.VehicleName = vehicle.Name;
                model.LicensePlates = vehicle.LicensePlates;
                model.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId)).Name;
                model.VehicleTypeLabel = (await _unitOfWork.VehicleTypeRepository.GetById(vehicle.VehicleTypeId)).Label;
            }
            var rate = await _unitOfWork.FeedbackForDriverRepository
                        .Query()
                        .Where(d => d.DriverId == model.Id)
                        .Select(x => (decimal)x.Rate)
                        .ToListAsync();


            if (rate.Count > 0)
            {
                model.FeedbackRating = rate.Average();
            }
            return model;
        }

        public async Task<List<DriverViewModel>> Search(DriverSearchModel model)
        {
            var driversList = await _unitOfWork.DriverRepository.Query()
                            .Where(x => model.FirstName == null || x.FirstName.Contains(model.FirstName))
                            .Where(x => model.LastName == null || x.LastName.Contains(model.LastName))
                            .Where(x => model.Phone == null || x.Phone.Contains(model.Phone))
                            .Where(x => model.DateOfBirth == null || (x.DateOfBirth != null && x.DateOfBirth.Value.Equals(model.DateOfBirth.Value)))
                            .Where(x => model.PartnerId == null || x.PartnerId.Equals(model.PartnerId.Value))
                            .Where(x => model.VehicleId == null || (x.VehicleId != null && x.VehicleId.Value.Equals(model.VehicleId.Value)))
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsDriverViewModel())
                            .ToListAsync();
            foreach (DriverViewModel x in driversList)
            {
                if (x.VehicleId != null)
                {
                    var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId.Value);
                    x.LicensePlates = vehicle.LicensePlates;
                    x.VehicleName = vehicle.Name;
                    x.VehicleTypeLabel = (await _unitOfWork.VehicleTypeRepository.GetById(vehicle.VehicleTypeId)).Label;
                    x.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId)).Name;
                }

                var rate = await _unitOfWork.FeedbackForDriverRepository
                        .Query()
                        .Where(d => d.DriverId == x.Id)
                        .Select(x => (decimal)x.Rate)
                        .ToListAsync();


                if (rate.Count > 0)
                {
                    x.FeedbackRating = rate.Average();
                }
            }
            return driversList;
        }

        public async Task<Response> Update(Guid id, UpdateDriverModel model, bool isAssignDriverForTrip = false, bool isSaveAsync = true)
        {
            var entity = await _unitOfWork.DriverRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            if (model.Phone != null && model.Phone != entity.Phone)
            {
                var isExistCode = await _unitOfWork.DriverRepository.Query().AnyAsync(x => x.Phone.Equals(model.Phone));
                if (isExistCode)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Tài xế đã tồn tại!"
                    };
                }
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

            // check update only vehicle for service type is booking service, can not update vehicle for bus service
            if (entity.VehicleId != null)
            {
                var serviceTypeFromVehicle = await _unitOfWork.VehicleRepository.GetById(entity.VehicleId.Value);
                var serviceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(serviceTypeFromVehicle.ServiceTypeId)).Name;
                if (serviceTypeName.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME) ||
                    (serviceTypeName.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME) && isAssignDriverForTrip == false)
                )
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể cập nhật tài xế cho dịch vụ thuê xe và đi xe buýt!"
                    };
                }

                if (model.VehicleId != null)
                {
                    var serviceTypeFromModelVehicle = await _unitOfWork.VehicleRepository.GetById(entity.VehicleId.Value);
                    var serviceTypeNameFromModelVehicle = (await _unitOfWork.ServiceTypeRepository.GetById(serviceTypeFromVehicle.ServiceTypeId)).Name;

                    if (serviceTypeName.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME) ||
                            (serviceTypeName.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME) && isAssignDriverForTrip == false)
                        )
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = "Không thể cập nhật tài xế cho dịch vụ thuê xe và đi xe buýt!"
                        };
                    }
                }

                entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId);

            }
            else
            {
                if (model.VehicleId != null)
                {
                    var serviceTypeFromVehicle = await _unitOfWork.VehicleRepository.GetById(model.VehicleId.Value);
                    var serviceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(serviceTypeFromVehicle.ServiceTypeId)).Name;
                    if (serviceTypeName.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME) ||
                        (serviceTypeName.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME) && isAssignDriverForTrip == false)
                    )
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = "Không thể cập nhật tài xế cho dịch vụ thuê xe và đi xe buýt!"
                        };
                    }
                    entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId);
                }
            }

            entity.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(entity.DateOfBirth, model.DateOfBirth);
            entity.Gender = UpdateTypeOfNotNullAbleObject<bool>(entity.Gender, model.Gender);
            entity.LastName = UpdateTypeOfNullAbleObject<string>(entity.LastName, model.LastName);
            entity.FirstName = UpdateTypeOfNullAbleObject<string>(entity.FirstName, model.FirstName);
            // entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            entity.ModifiedDate = DateTime.Now;
            _unitOfWork.DriverRepository.Update(entity);

            if (isSaveAsync)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        // Get vehicles list for service type is booking service
        public async Task<List<VehicleViewModel>> GetVehicleListDropdownOptions(VehicleDropdownOptionsModel model)
        {
            var vehiclesList = await _unitOfWork.VehicleRepository
                            .Query()
                            .Where(x => x.PartnerId == model.PartnerId)
                            .Where(x => x.ServiceTypeId == model.ServiceTypeId)
                            .Where(x => x.Status == 1)
                            .Select(x => x.AsVehicleViewModel())
                            .ToListAsync();

            for (int i = 0; i < vehiclesList.Count; i++)
            {
                // Has been checked vehicle to be assigned for other drivers yet?
                var check = await _unitOfWork.DriverRepository
                            .Query()
                            .Where(x => x.VehicleId == vehiclesList[i].Id)
                            .FirstOrDefaultAsync();

                if (check != null)
                {
                    vehiclesList.RemoveAt(i);
                }
            }

            foreach (VehicleViewModel x in vehiclesList)
            {
                x.VehicleTypeName = (await _unitOfWork.VehicleTypeRepository.GetById(x.VehicleTypeId)).Label;
            }

            return vehiclesList;
        }

        public async Task<SearchResultViewModel<DriverHistoryViewModel>> GetDriverHistory(DriverTripHistorySearchModel model)
        {
            var driverHistoriesOfBusServiceList = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Join(_unitOfWork.TripRepository.Query(),
                                        customerTrip => customerTrip.Trip != null ? customerTrip.TripId.Value : customerTrip.TripId,
                                        trip => trip.TripId,
                                        (customerTrip, trip) => new { customerTrip, trip }
                                    )
                                    .Join(_unitOfWork.RouteRepository.Query(),
                                        customerTrip_Trip => customerTrip_Trip.trip.RouteId,
                                        route => route.RouteId,
                                        (customerTrip_Trip, route) => new { customerTrip_Trip, route }
                                    )
                                    .Join(_unitOfWork.DriverRepository.Query(),
                                        customerTrip_Trip_Route => customerTrip_Trip_Route.customerTrip_Trip.trip.DriverId,
                                        driver => driver.DriverId,
                                        (customerTrip_Trip_Route, driver) => new { customerTrip_Trip_Route, driver }
                                    )
                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                        customerTrip_Trip_Route_Driver => customerTrip_Trip_Route_Driver.driver.VehicleId,
                                        vehicle => vehicle.VehicleId,
                                        (customerTrip_Trip_Route_Driver, vehicle) => new { customerTrip_Trip_Route_Driver, vehicle }
                                    )
                                    .Join(_unitOfWork.VehicleTypeRepository.Query(),
                                        customerTrip_Trip_Route_Driver_Vehicle => customerTrip_Trip_Route_Driver_Vehicle.vehicle.VehicleTypeId,
                                        vehicleType => vehicleType.VehicleTypeId,
                                        (customerTrip_Trip_Route_Driver_Vehicle, vehicleType) => new { customerTrip_Trip_Route_Driver_Vehicle, vehicleType }
                                    )
                                    .Join(_unitOfWork.ServiceTypeRepository.Query(),
                                        customerTrip_Trip_Route_Driver_Vehicle_VehicleType => customerTrip_Trip_Route_Driver_Vehicle_VehicleType.customerTrip_Trip_Route_Driver_Vehicle.vehicle.ServiceTypeId,
                                        serviceType => serviceType.ServiceTypeId,
                                        (customerTrip_Trip_Route_Driver_Vehicle_VehicleType, serviceType) => new { customerTrip_Trip_Route_Driver_Vehicle_VehicleType, serviceType }
                                    )
                                    .Join(_unitOfWork.FeedbackForDriverRepository.Query(),
                                        customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType => customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                                                            .customerTrip_Trip_Route_Driver
                                                                                            .customerTrip_Trip_Route
                                                                                            .customerTrip_Trip
                                                                                            .customerTrip
                                                                                            .CustomerTripId,
                                        feedback => feedback.CustomerTripId,
                                        (customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType, feedback) => new DriverTripHistoryViewModel()
                                        {
                                            CustomerTripId = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .customerTrip_Trip_Route
                                                            .customerTrip_Trip
                                                            .customerTrip
                                                            .CustomerTripId,

                                            VehicleId = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .vehicle
                                                        .VehicleId,

                                            VehicleName = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .vehicle
                                                        .Name,

                                            LicensePlates = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .vehicle
                                                            .LicensePlates,

                                            ServiceTypeId = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .serviceType
                                                            .ServiceTypeId,

                                            ServiceTypeName = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .serviceType
                                                            .Name,

                                            Id = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .driver
                                                        .DriverId,

                                            PartnerId = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .driver
                                                        .PartnerId,

                                            VehicleTypeLabel = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                                .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                                .vehicleType
                                                                .Label,

                                            FirstName = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .FirstName,

                                            LastName = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .LastName,

                                            PhotoUrl = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .PhotoUrl,

                                            Gender = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .Gender,

                                            Phone = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .Phone,

                                            DateOfBirth = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .DateOfBirth,

                                            CreatedDate = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .CreatedDate,

                                            ModifiedDate = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                            .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                            .customerTrip_Trip_Route_Driver_Vehicle
                                                            .customerTrip_Trip_Route_Driver
                                                            .driver
                                                            .ModifiedDate,

                                            RouteId = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                    .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                    .customerTrip_Trip_Route_Driver_Vehicle
                                                    .customerTrip_Trip_Route_Driver
                                                    .customerTrip_Trip_Route
                                                    .route
                                                    .RouteId,

                                            RouteName = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .customerTrip_Trip_Route
                                                        .route
                                                        .Name,

                                            FeedbackRating = feedback.Rate,

                                            Distance = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .customerTrip_Trip_Route
                                                        .customerTrip_Trip
                                                        .customerTrip
                                                        .Distance != null
                                                        ?
                                                       customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .customerTrip_Trip_Route
                                                        .customerTrip_Trip
                                                        .customerTrip
                                                        .Distance
                                                        .Value
                                                        :
                                                       null,

                                            CreatedDateDriverHis = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .customerTrip_Trip_Route
                                                        .customerTrip_Trip
                                                        .customerTrip
                                                        .CreatedDate,

                                            ModifiedDateDriverHis = customerTrip_Trip_Route_Driver_Vehicle_VehicleType_ServiceType
                                                        .customerTrip_Trip_Route_Driver_Vehicle_VehicleType
                                                        .customerTrip_Trip_Route_Driver_Vehicle
                                                        .customerTrip_Trip_Route_Driver
                                                        .customerTrip_Trip_Route
                                                        .customerTrip_Trip
                                                        .customerTrip
                                                        .ModifiedDate
                                        }
                                    )
                                    .Where(x => x.Id == model.DriverId)
                                    .ToListAsync();

            var driverHistoriesOfBookingServiceList = await _unitOfWork.CustomerTripRepository
                                                    .Query()
                                                    .Where(x => x.TripId == null)
                                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                                        customerTrip => customerTrip.VehicleId,
                                                        vehicle => vehicle.VehicleId,
                                                        (customerTrip, vehicle) => new { customerTrip, vehicle }
                                                    )
                                                    .Join(_unitOfWork.VehicleTypeRepository.Query(),
                                                        customerTrip_Vehicle => customerTrip_Vehicle.vehicle.VehicleTypeId,
                                                        vehicleType => vehicleType.VehicleTypeId,
                                                        (customerTrip_Vehicle, vehicleType) => new { customerTrip_Vehicle, vehicleType }
                                                    )
                                                    .Join(_unitOfWork.DriverRepository.Query(),
                                                        customerTrip_Vehicle_VehicleType => customerTrip_Vehicle_VehicleType.customerTrip_Vehicle.vehicle.VehicleId,
                                                        driver => driver.VehicleId != null ? driver.VehicleId.Value : driver.VehicleId,
                                                        (customerTrip_Vehicle_VehicleType, driver) => new { customerTrip_Vehicle_VehicleType, driver }
                                                    )
                                                    .Join(_unitOfWork.ServiceTypeRepository.Query(),
                                                        customerTrip_Vehicle_VehicleType_Driver => customerTrip_Vehicle_VehicleType_Driver.customerTrip_Vehicle_VehicleType.customerTrip_Vehicle.vehicle.ServiceTypeId,
                                                        serviceType => serviceType.ServiceTypeId,
                                                        (customerTrip_Vehicle_VehicleType_Driver, serviceType) => new { customerTrip_Vehicle_VehicleType_Driver, serviceType }
                                                    )
                                                    .Join(_unitOfWork.FeedbackForDriverRepository.Query(),
                                                        customerTrip_Vehicle_VehicleType_Driver_ServiceType => customerTrip_Vehicle_VehicleType_Driver_ServiceType.customerTrip_Vehicle_VehicleType_Driver.customerTrip_Vehicle_VehicleType.customerTrip_Vehicle.customerTrip.CustomerTripId,
                                                        feedback => feedback.CustomerTripId,
                                                        (customerTrip_Vehicle_VehicleType_Driver_ServiceType, feedback) => new DriverTripHistoryViewModel()
                                                        {
                                                            CustomerTripId = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                                .customerTrip_Vehicle_VehicleType_Driver
                                                                                .customerTrip_Vehicle_VehicleType
                                                                                .customerTrip_Vehicle
                                                                                .customerTrip
                                                                                .CustomerTripId,

                                                            VehicleId = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .customerTrip_Vehicle_VehicleType
                                                                        .customerTrip_Vehicle
                                                                        .vehicle
                                                                        .VehicleId,

                                                            VehicleName = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                            .customerTrip_Vehicle_VehicleType_Driver
                                                                            .customerTrip_Vehicle_VehicleType
                                                                            .customerTrip_Vehicle
                                                                            .vehicle
                                                                            .Name,

                                                            LicensePlates = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                            .customerTrip_Vehicle_VehicleType_Driver
                                                                            .customerTrip_Vehicle_VehicleType
                                                                            .customerTrip_Vehicle
                                                                            .vehicle
                                                                            .LicensePlates,

                                                            ServiceTypeId = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                            .serviceType
                                                                            .ServiceTypeId,

                                                            ServiceTypeName = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                                .serviceType
                                                                                .Name,

                                                            Id = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .DriverId,

                                                            PartnerId = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .PartnerId,

                                                            VehicleTypeLabel = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                                .customerTrip_Vehicle_VehicleType_Driver
                                                                                .customerTrip_Vehicle_VehicleType
                                                                                .vehicleType
                                                                                .Label,

                                                            FirstName = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .FirstName,

                                                            LastName = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .LastName,

                                                            PhotoUrl = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .PhotoUrl,

                                                            Gender = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .Gender,

                                                            Phone = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .Phone,

                                                            DateOfBirth = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .DateOfBirth,

                                                            CreatedDate = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .CreatedDate,

                                                            ModifiedDate = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .ModifiedDate,

                                                            Status = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .driver
                                                                        .Status,

                                                            Distance = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .customerTrip_Vehicle_VehicleType
                                                                        .customerTrip_Vehicle
                                                                        .customerTrip
                                                                        .Distance != null
                                                                        ?
                                                                        customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .customerTrip_Vehicle_VehicleType
                                                                        .customerTrip_Vehicle
                                                                        .customerTrip
                                                                        .Distance.Value
                                                                        :
                                                                        null,

                                                            FeedbackRating = feedback.Rate,

                                                            CreatedDateDriverHis = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .customerTrip_Vehicle_VehicleType
                                                                        .customerTrip_Vehicle
                                                                        .customerTrip
                                                                        .CreatedDate,

                                                            ModifiedDateDriverHis = customerTrip_Vehicle_VehicleType_Driver_ServiceType
                                                                        .customerTrip_Vehicle_VehicleType_Driver
                                                                        .customerTrip_Vehicle_VehicleType
                                                                        .customerTrip_Vehicle
                                                                        .customerTrip
                                                                        .ModifiedDate,
                                                        }
                                                    )
                                                    .Where(x => x.Id == model.DriverId)
                                                    .ToListAsync();

            List<DriverTripHistoryViewModel> list = new List<DriverTripHistoryViewModel>();
            list.AddRange(driverHistoriesOfBusServiceList);
            list.AddRange(driverHistoriesOfBookingServiceList);

            var listAfterSorting = GetListAfterSorting(list, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);

            DriverHistoryViewModel driverHis = new DriverHistoryViewModel();
            driverHis.History = new List<History>();
            List<decimal> feedbacksList = new List<decimal>();
            for (int i = 0; i < listItemsAfterPaging.Count; i++)
            {
                if (i == 0)
                {
                    driverHis.Driver = new DriverViewModel()
                    {
                        Id = listItemsAfterPaging[i].Id,
                        PartnerId = listItemsAfterPaging[i].PartnerId,
                        VehicleId = listItemsAfterPaging[i].VehicleId != null ? listItemsAfterPaging[i].VehicleId.Value : null,
                        VehicleName = listItemsAfterPaging[i].VehicleName,
                        VehicleTypeLabel = listItemsAfterPaging[i].VehicleTypeLabel,
                        LicensePlates = listItemsAfterPaging[i].LicensePlates,
                        ServiceTypeName = listItemsAfterPaging[i].ServiceTypeName,
                        FirstName = listItemsAfterPaging[i].FirstName,
                        LastName = listItemsAfterPaging[i].LastName,
                        PhotoUrl = listItemsAfterPaging[i].PhotoUrl,
                        Gender = listItemsAfterPaging[i].Gender,
                        Phone = listItemsAfterPaging[i].Phone,
                        DateOfBirth = listItemsAfterPaging[i].DateOfBirth != null ? listItemsAfterPaging[i].DateOfBirth.Value : null,
                        CreatedDate = listItemsAfterPaging[i].CreatedDate,
                        ModifiedDate = listItemsAfterPaging[i].ModifiedDate,
                        Status = listItemsAfterPaging[i].Status
                    };
                }

                var history = new History()
                {
                    CustomerTripId = listItemsAfterPaging[i].CustomerTripId,
                    VehicleId = listItemsAfterPaging[i].VehicleId.Value,
                    VehicleName = listItemsAfterPaging[i].VehicleName,
                    LicensePlates = listItemsAfterPaging[i].LicensePlates,
                    ServiceTypeId = listItemsAfterPaging[i].ServiceTypeId,
                    ServiceTypeName = listItemsAfterPaging[i].ServiceTypeName,
                    RouteId = listItemsAfterPaging[i].RouteId != null ? listItemsAfterPaging[i].RouteId.Value : null,
                    RouteName = listItemsAfterPaging[i].RouteName,
                    FeedbackDriverRating = (int)listItemsAfterPaging[i].FeedbackRating,
                    Distance = listItemsAfterPaging[i].Distance != null ? listItemsAfterPaging[i].Distance.Value : null,
                    CreatedDate = listItemsAfterPaging[i].CreatedDateDriverHis,
                    ModifiedDate = listItemsAfterPaging[i].ModifiedDateDriverHis,
                };

                driverHis.History.Add(history);
                if (history.FeedbackDriverRating != 0)
                {
                    feedbacksList.Add(history.FeedbackDriverRating);
                }
            }

            if (driverHis.Driver != null)
            {
                driverHis.Driver.FeedbackRating = feedbacksList.Average();
            }

            List<DriverHistoryViewModel> driverHisList = new List<DriverHistoryViewModel>();
            driverHisList.Add(driverHis);

            SearchResultViewModel<DriverHistoryViewModel> result = null;
            result = new SearchResultViewModel<DriverHistoryViewModel>()
            {
                Items = driverHisList,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }




        //--------------------------------------------------

        private async Task<Response> CheckReferenceToOther(Guid id)
        {
            var checkExistedReferenceToTrip = await _unitOfWork.TripRepository
                                            .Query()
                                            .AnyAsync(
                                                            x => x.DriverId == id
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
    }
}