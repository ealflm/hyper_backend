using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class TripManagementService : BaseService, ITripManagementService
    {
        public TripManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreateTrip(CreateTripModel model)
        {
            var validatorResult = await CheckValidationData(model);
            if (validatorResult.StatusCode != 0)
            {
                return validatorResult;
            }

            var existedTrip = await _unitOfWork.TripRepository
                            .Query()
                            .AnyAsync(x => x.RouteId == model.RouteId.Value &&
                                        x.VehicleId == model.VehicleId &&
                                        (DateTime.Compare(x.TimeStart, model.TimeStart.Value) <= 0 && // TimeStart between Start & End
                                        DateTime.Compare(x.TimeEnd, model.TimeStart.Value) >= 0) ||
                                        (DateTime.Compare(x.TimeStart, model.TimeEnd.Value) <= 0 && // TimeEnd between Start & End
                                        DateTime.Compare(x.TimeEnd, model.TimeEnd.Value) >= 0));
            if (existedTrip)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Tuyến đã tồn tại!"
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
                TimeStart = model.TimeStart.Value,
                TimeEnd = model.TimeEnd.Value,
                Status = 1
            };
            await _unitOfWork.TripRepository.Add(newTrip);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo tuyến thành công!"
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

            var result = await CheckReferenceToOther(id);
            if (result.StatusCode != 0)
            {
                return result;
            }

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
                                    .Where(x => model.TripName == null || x.TripName.Contains(model.TripName))
                                    .Select(x => x.AsTripViewModel())
                                    .ToListAsync();
                tripsList.AddRange(trips);
            }

            var listAfterSorting = GetListAfterSorting(tripsList, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var t in listItemsAfterPaging)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(t.VehicleId);
                t.LicensePlates = vehicle.LicensePlates;
                t.VehicleName = vehicle.Name;
                var driver = await _unitOfWork.DriverRepository.GetById(t.DriverId);
                t.DriverFirstName = driver.FirstName;
                t.DriverLastName = driver.LastName;
                t.DriverPhotoUrl = driver.PhotoUrl;
            }
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
                    Message = "Không tìm thấy!"
                };
            }

            if ((model.RouteId != null && entity.RouteId != model.RouteId.Value) ||
                (model.VehicleId != null && entity.VehicleId != model.VehicleId.Value))
            {
                var existedTrip = await _unitOfWork.TripRepository
                           .Query()
                           .AnyAsync(x => x.RouteId == model.RouteId.Value &&
                                       x.VehicleId == model.VehicleId &&
                                       (DateTime.Compare(x.TimeStart, model.TimeStart.Value) <= 0 && // TimeStart between Start & End
                                       DateTime.Compare(x.TimeEnd, model.TimeStart.Value) >= 0) ||
                                       (DateTime.Compare(x.TimeStart, model.TimeEnd.Value) <= 0 && // TimeEnd between Start & End
                                       DateTime.Compare(x.TimeEnd, model.TimeEnd.Value) >= 0));
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
            entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId.Value);
            entity.DriverId = UpdateTypeOfNotNullAbleObject<Guid>(entity.DriverId, model.DriverId.Value);
            entity.TripName = UpdateTypeOfNullAbleObject<string>(entity.TripName, model.TripName);
            entity.DayOfWeek = UpdateTypeOfNotNullAbleObject<int>(entity.DayOfWeek, model.DayOfWeek);
            entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
            entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
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
    }
}
