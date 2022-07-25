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
        public readonly string Bus = "Đi xe theo chuyến";
        public readonly string Booking = "Đặt xe";
        public readonly string Renting = "Thuê xe";

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

        public async Task<Response> Update(Guid id, UpdateDriverModel model, bool isSaveAsync = true)
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
                if (!serviceTypeName.Contains(Booking))
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Cập nhật xe không hợp lệ. Chỉ có thể cập nhật xe với dịch vụ đặt xe!"
                    };
                }
                entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId);

            }
            else
            {
                if (model.VehicleId != null)
                {
                    var serviceTypeFromVehicle = await _unitOfWork.VehicleRepository.GetById(model.VehicleId.Value);
                    var serviceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(serviceTypeFromVehicle.ServiceTypeId)).Name;
                    if (!serviceTypeName.Contains(Booking))
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = "Cập nhật xe không hợp lệ. Chỉ có thể cập nhật xe với dịch vụ đặt xe!"
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

        public async Task<SearchResultViewModel<DriverTripHistoryViewModel>> GetDriverHistory(DriverTripHistorySearchModel model)
        {
            var driverHistoriesList = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Join(_unitOfWork.RouteRepository.Query(),
                                        customerTrip => customerTrip.RouteId,
                                        route => route.RouteId,
                                        (customerTrip, route) => new { customerTrip, route }
                                    )
                                    .Join(_unitOfWork.TripRepository.Query(),
                                        customerTrip_Route => customerTrip_Route.route.RouteId,
                                        trip => trip.RouteId,
                                        (customerTrip_Route, trip) => new { customerTrip_Route, trip }
                                    )
                                    .Join(_unitOfWork.DriverRepository.Query(),
                                        customerTrip_Route_Trip => customerTrip_Route_Trip.trip.DriverId,
                                        driver => driver.DriverId,
                                        (customerTrip_Route_Trip, driver) => new { customerTrip_Route_Trip, driver }
                                    )
                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                        customerTrip_Route_Trip_Driver => customerTrip_Route_Trip_Driver.driver.VehicleId,
                                        vehicle => vehicle.VehicleId,
                                        (customerTrip_Route_Trip_Driver, vehicle) => new DriverTripHistoryViewModel()
                                        {
                                            VehicleId = vehicle.VehicleId,
                                            VehicleName = vehicle.Name,
                                            LicensePlates = vehicle.LicensePlates,
                                            DriverId = customerTrip_Route_Trip_Driver.driver.DriverId,
                                            DriverFirstName = customerTrip_Route_Trip_Driver.driver.FirstName,
                                            DriverLastName = customerTrip_Route_Trip_Driver.driver.LastName,
                                            RouteId = customerTrip_Route_Trip_Driver.customerTrip_Route_Trip.customerTrip_Route.route.RouteId,
                                            RouteName = customerTrip_Route_Trip_Driver.customerTrip_Route_Trip.customerTrip_Route.route.Name,
                                            Distance = customerTrip_Route_Trip_Driver.customerTrip_Route_Trip.customerTrip_Route.customerTrip.Distance != null
                                                        ?
                                                       customerTrip_Route_Trip_Driver.customerTrip_Route_Trip.customerTrip_Route.customerTrip.Distance.Value
                                                        :
                                                       null,
                                            CreatedDate = customerTrip_Route_Trip_Driver.customerTrip_Route_Trip.customerTrip_Route.customerTrip.CreatedDate,
                                            ModifiedDate = customerTrip_Route_Trip_Driver.customerTrip_Route_Trip.customerTrip_Route.customerTrip.ModifiedDate
                                        }
                                    )
                                    .Where(x => x.DriverId == model.DriverId)
                                    .ToListAsync();

            var listAfterSorting = GetListAfterSorting(driverHistoriesList, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<DriverTripHistoryViewModel> result = null;
            result = new SearchResultViewModel<DriverTripHistoryViewModel>()
            {
                Items = listItemsAfterPaging,
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