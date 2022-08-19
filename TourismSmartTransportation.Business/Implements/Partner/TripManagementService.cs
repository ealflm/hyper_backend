using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class TripManagementService : BaseService, ITripManagementService
    {
        private readonly IDriverManagementService _driverService;

        public TripManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient,
                                        IDriverManagementService driverService) : base(unitOfWork, blobServiceClient)
        {
            _driverService = driverService;
        }

        public async Task<Response> CreateTrip(CreateTripModel model)
        {
            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            // Thời gian bắt đầu của trip mới phải lớn hơn thời gian kết thúc của một trip đã tồn tại
            var existedTrip = await _unitOfWork.TripRepository
                            .Query()
                            .AnyAsync( // Kiểm trả trip đã tồn tại
                                       // x => x.RouteId == model.RouteId.Value && // Cùng tuyến đường
                                x => x.VehicleId == model.VehicleId.Value && // Cùng phương tiện
                                x.DayOfWeek == model.DayOfWeek.Value && // Cùng ngày trong tuần
                                (
                                    (
                                        x.TimeEnd.CompareTo(model.TimeStart) == 0 // Trường hợp thời gian bắt đầu bằng với thời gian kết thúc của trip đã tồn tại
                                    )
                                    ||
                                    (
                                        x.TimeStart.CompareTo(model.TimeStart) <= 0 && // Trường hợp thời gian bắt đầu nằm trong khoảng thời gian của trip đã tồn tại
                                        model.TimeStart.CompareTo(x.TimeEnd) <= 0
                                    )
                                    ||
                                    (
                                        x.TimeStart.CompareTo(model.TimeEnd) <= 0 && // Trường hợp thời gian kết thúc nằm trong khoảng thời gian của trip đã tồn tại
                                        model.TimeEnd.CompareTo(x.TimeEnd) <= 0
                                    )
                                )

                            // &&
                            // (
                            //     (
                            //         DateTime.Compare(x.TimeStart, model.TimeStart.Value) <= 0 && // TimeStart between Start & End
                            //         DateTime.Compare(x.TimeEnd, model.TimeStart.Value) >= 0
                            //     )
                            //     ||
                            //     (
                            //         DateTime.Compare(x.TimeStart, model.TimeEnd.Value) <= 0 && // TimeEnd between Start & End
                            //         DateTime.Compare(x.TimeEnd, model.TimeEnd.Value) >= 0
                            //     )
                            // )
                            );

            if (existedTrip)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Tạo mới không thành công. Tuyến đã tồn tại hoặc xe đã được đăng ký cho tuyến khác"
                };
            }

            var newTrip = new Trip()
            {
                TripId = Guid.NewGuid(),
                DriverId = model.DriverId.Value,
                VehicleId = model.VehicleId.Value,
                RouteId = model.RouteId.Value,
                TripName = model.TripName,
                DayOfWeek = model.DayOfWeek.Value,
                TimeStart = model.TimeStart,
                TimeEnd = model.TimeEnd,
                Week = model.Week,
                Status = 1
            };
            await _unitOfWork.TripRepository.Add(newTrip);

            UpdateDriverModel updateDriver = new UpdateDriverModel() { VehicleId = newTrip.VehicleId };
            var driverResponse = await _driverService.Update(
                                                            id: newTrip.DriverId,
                                                            model: updateDriver,
                                                            isAssignDriverForTrip: true,
                                                            isSaveAsync: false
                                                        );
            if (driverResponse.StatusCode != 201)
            {
                return new()
                {
                    StatusCode = 500,
                    Message = "Tạo tuyến thất bại. Thêm tài xế cho tuyến không thành công!"
                };
            }

            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới tuyến thành công!"
            };
        }

        public async Task<Response> DeleteTrip(Guid id)
        {
            var trip = await _unitOfWork.TripRepository.GetById(id);
            if (trip == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy tuyến!"
                };
            }

            // var result = await CheckReferenceToOther(id);
            // if (result.StatusCode != 0)
            // {
            //     return result;
            // }

            trip.Status = 0;
            _unitOfWork.TripRepository.Update(trip);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<TripViewModel> GetTripById(Guid id)
        {
            var trip = await _unitOfWork.TripRepository.GetById(id);
            var model = trip.AsTripViewModel();
            model.LicensePlates = (await _unitOfWork.VehicleRepository.GetById(model.VehicleId)).LicensePlates;
            return model;
        }

        public async Task<SearchResultViewModel<TripViewModel>> GetTripsList(TripSearchModel model)
        {
            var routesList = await _unitOfWork.RouteRepository
                        .Query()
                        .Where(x => model.PartnerId == null || x.PartnerId == model.PartnerId.Value)
                        .ToListAsync();
            List<TripViewModel> tripsList = new List<TripViewModel>();
            foreach (var route in routesList)
            {
                var trips = await _unitOfWork.TripRepository
                                    .Query()
                                    .Where(x => x.RouteId == route.RouteId)
                                    .Where(x => model.VehicleId == null || model.VehicleId.Value == x.VehicleId)
                                    .Where(x => model.Status == null || model.Status.Value == x.Status)
                                    .Where(x => model.TripName == null || x.TripName.Contains(model.TripName))
                                    .Where(x => model.Week == null || x.Week.Equals(model.Week))
                                     .Where(x => model.RouteId == null || x.RouteId.Equals(model.RouteId.Value))
                                     .Where(x => model.DayOfWeek == null || x.DayOfWeek==model.DayOfWeek.Value)
                                    .Join(_unitOfWork.RouteRepository.Query(),
                                        trip => trip.RouteId,
                                        route => route.RouteId,
                                        (trip, route) => new { trip, route }
                                    )
                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                        tripRoute => tripRoute.trip.VehicleId,
                                        vehicle => vehicle.VehicleId,
                                        (tripRoute, vehicle) => new { tripRoute, vehicle }
                                    )
                                    .Join(_unitOfWork.DriverRepository.Query(),
                                        tripRouteVehicle => tripRouteVehicle.tripRoute.trip.DriverId,
                                        driver => driver.DriverId,
                                        (tripRouteVehicle, driver) => new TripViewModel()
                                        {
                                            TripId = tripRouteVehicle.tripRoute.trip.TripId,
                                            TripName = tripRouteVehicle.tripRoute.trip.TripName,
                                            RouteId = tripRouteVehicle.tripRoute.route.RouteId,
                                            RouteName = tripRouteVehicle.tripRoute.route.Name,
                                            DriverId = driver.DriverId,
                                            DriverFirstName = driver.FirstName,
                                            DriverLastName = driver.LastName,
                                            DriverPhotoUrl = driver.PhotoUrl,
                                            VehicleId = tripRouteVehicle.vehicle.VehicleId,
                                            VehicleName = tripRouteVehicle.vehicle.Name,
                                            LicensePlates = tripRouteVehicle.vehicle.LicensePlates,
                                            DayOfWeek = tripRouteVehicle.tripRoute.trip.DayOfWeek,
                                            TimeStart = tripRouteVehicle.tripRoute.trip.TimeStart,
                                            TimeEnd = tripRouteVehicle.tripRoute.trip.TimeEnd,
                                            Week= tripRouteVehicle.tripRoute.trip.Week,
                                            Status = tripRouteVehicle.tripRoute.trip.Status
                                        }
                                    )
                                    .ToListAsync();
                tripsList.AddRange(trips);
            }

            var listAfterSorting = GetListAfterSorting(tripsList, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            // foreach (var t in listItemsAfterPaging)
            // {
            //     var vehicle = await _unitOfWork.VehicleRepository.GetById(t.VehicleId);
            //     t.LicensePlates = vehicle.LicensePlates;
            //     t.VehicleName = vehicle.Name;

            //     var driver = await _unitOfWork.DriverRepository.GetById(t.DriverId);
            //     t.DriverFirstName = driver.FirstName;
            //     t.DriverLastName = driver.LastName;
            //     t.DriverPhotoUrl = driver.PhotoUrl;
            // }
            SearchResultViewModel<TripViewModel> result = null;
            result = new SearchResultViewModel<TripViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdateTrip(Guid id, UpdateTripModel model)
        {
            var entity = await _unitOfWork.TripRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy tuyến!"
                };
            }

            bool isCheckExistedTrip = true;
            if (
                (
                    model.RouteId == null &&
                    model.VehicleId == null &&
                    model.DayOfWeek == null &&
                    model.TimeStart == null &&
                    model.TimeEnd == null
                )
                ||
                (
                    model.RouteId.Value == entity.RouteId &&
                    model.VehicleId.Value == entity.VehicleId &&
                    model.DayOfWeek.Value == entity.DayOfWeek &&
                    model.TimeStart == entity.TimeStart &&
                    model.TimeEnd == entity.TimeEnd
                )
            )
            {
                isCheckExistedTrip = false;
            }

            if (isCheckExistedTrip)
            {
                var existedTrip = await _unitOfWork.TripRepository
                       .Query()
                       .Where(x => x.TripId != entity.TripId)
                       .AnyAsync( // Kiểm trả trip đã tồn tại
                            x => x.RouteId == (model.RouteId != null ? model.RouteId.Value : entity.RouteId) && // Cùng tuyến đường
                            x.VehicleId == (model.VehicleId != null ? model.VehicleId.Value : entity.VehicleId) && // Cùng phương tiện
                            x.DayOfWeek == (model.DayOfWeek != null ? model.DayOfWeek.Value : entity.DayOfWeek) && // Cùng ngày trong tuần
                            (
                                (
                                    x.TimeEnd.CompareTo(model.TimeStart) == 0 // Trường hợp thời gian bắt đầu bằng với thời gian kết thúc của trip đã tồn tại
                                )
                                ||
                                (
                                    x.TimeStart.CompareTo(model.TimeStart) <= 0 && // Trường hợp thời gian bắt đầu nằm trong khoảng thời gian của trip đã tồn tại
                                    model.TimeStart.CompareTo(x.TimeEnd) <= 0
                                )
                                ||
                                (
                                    x.TimeStart.CompareTo(model.TimeEnd) <= 0 && // Trường hợp thời gian kết thúc nằm trong khoảng thời gian của trip đã tồn tại
                                    model.TimeEnd.CompareTo(x.TimeEnd) <= 0
                                )
                            )
                        );

                if (existedTrip)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Tuyến đã tồn tại!"
                    };
                }
            }

            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            entity.RouteId = UpdateTypeOfNotNullAbleObject<Guid>(entity.RouteId, model.RouteId);
            entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId);
            entity.DriverId = UpdateTypeOfNotNullAbleObject<Guid>(entity.DriverId, model.DriverId);
            entity.TripName = UpdateTypeOfNullAbleObject<string>(entity.TripName, model.TripName);
            entity.DayOfWeek = UpdateTypeOfNotNullAbleObject<int>(entity.DayOfWeek, model.DayOfWeek);
            entity.TimeStart = UpdateTypeOfNullAbleObject<string>(entity.TimeStart, model.TimeStart);
            entity.TimeEnd = UpdateTypeOfNullAbleObject<string>(entity.TimeEnd, model.TimeEnd);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.TripRepository.Update(entity);
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

        public async Task<List<VehicleViewModel>> GetVehicleListDropdownOptions(VehicleDropdownOptionsTripModel model)
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
                var trip = await _unitOfWork.TripRepository
                            .Query()
                            .Where(x => x.VehicleId == vehiclesList[i].Id)
                            .FirstOrDefaultAsync();

                if (trip != null && DateTime.Now.CompareTo(trip.TimeEnd) <= 0)
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

        public async Task<List<DriverViewModel>> GetDriverListDropdownOptions(DriverDropdownOptionsTripModel model)
        {
            var driversList = await _unitOfWork.DriverRepository
                            .Query()
                            .Where(x => x.PartnerId == model.PartnerId)
                            .Where(x => x.Status == 1)
                            .Select(x => x.AsDriverViewModel())
                            .ToListAsync();

            for (int i = 0; i < driversList.Count; i++)
            {
                if (driversList[i].VehicleId != null)
                {
                    var serviceType = await _unitOfWork.VehicleRepository.GetById(driversList[i].VehicleId.Value);
                    driversList[i].ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(serviceType.ServiceTypeId)).Name;

                    if (!driversList[i].ServiceTypeName.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME))
                    {
                        driversList.RemoveAt(i);
                    }
                }
            }

            return driversList;
        }

        //------------------------------------------------------------------
        private async Task<Response> CheckValidationData(TripModel model)
        {
            // Check Route
            if (model.RouteId != null)
            {
                var Route = await _unitOfWork.RouteRepository.GetById(model.RouteId.Value);
                if (Route == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Tuyến đường không tồn tại"
                    };
                }
                else if (Route.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với tuyến đường đã bị vô hiệu hóa"
                    };
                }
            }

            // Check driver
            if (model.DriverId != null)
            {
                var driver = await _unitOfWork.DriverRepository.GetById(model.DriverId.Value);
                if (driver == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Loại dịch vụ không tồn tại"
                    };
                }
                else if (driver.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với loại dịch vụ đã bị vô hiệu hóa"
                    };
                }
            }

            // Check vehicle
            if (model.VehicleId != null)
            {
                var vehicleType = await _unitOfWork.VehicleRepository.GetById(model.VehicleId.Value);
                if (vehicleType == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Phương tiện không tồn tại"
                    };
                }
                else if (vehicleType.Status == 0)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Không thể thực hiện thao tác với phương tiện đã bị vô hiệu hóa"
                    };
                }
            }

            // Data is availiable
            return new()
            {
                StatusCode = 0, // No Error
            };
        }

        public async Task<Response> CopyTrip(CopyTripModel model)
        {
            var checkTrip = await _unitOfWork.TripRepository.Query().Where(x => x.Week.Equals(model.ToWeek)).FirstOrDefaultAsync();
            if (checkTrip == null)
            {
                var trips = await _unitOfWork.TripRepository.Query().Where(x => x.Week.Equals(model.FromWeek)).ToListAsync();
                foreach (Trip trip in trips)
                {
                    var driver = await _unitOfWork.DriverRepository.GetById(trip.DriverId);
                    if (driver.PartnerId.Equals(model.PartnerId))
                    {
                        trip.TripId = Guid.NewGuid();
                        trip.Week = model.ToWeek;
                        await _unitOfWork.TripRepository.Add(trip);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return new()
                {
                    StatusCode = 200,
                    Message = "Sao chép thành công"
                };
            }
            else
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Sao chép thất bại"
                };
            }
        }
    }
}
