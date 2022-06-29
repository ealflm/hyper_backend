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

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class DriverManagementService : AccountService, IDriverManagementService
    {
        private readonly string MESSAGE = "Dang nhap bang SDT da dang ky voi MAT KHAU: ";

        public DriverManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, Credentials credentials, HttpClient client) : base(unitOfWork, blobServiceClient, credentials, client)
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
                VehicleId = model.VehicleId,
                Password = passwordHash,
                Salt = passwordSalt,
                Status = 1
            };

            await _unitOfWork.DriverRepository.Add(driver);
            await _unitOfWork.SaveChangesAsync();
            SendSMS(driver.Phone, MESSAGE + password);
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
            var vehicle = await _unitOfWork.VehicleRepository.GetById(model.VehicleId);
            model.VehicleName = vehicle.Name;
            model.VehicleTypeLabel = (await _unitOfWork.VehicleTypeRepository.GetById(vehicle.VehicleTypeId)).Label;
            return model;

        }

        public async Task<List<DriverViewModel>> Search(DriverSearchModel model)
        {
            var entity = await _unitOfWork.DriverRepository.Query()
                            .Where(x => model.FirstName == null || x.FirstName.Contains(model.FirstName))
                            .Where(x => model.LastName == null || x.LastName.Contains(model.LastName))
                            .Where(x => model.Phone == null || x.Phone.Contains(model.Phone))
                            .Where(x => model.DateOfBirth == null || x.DateOfBirth.Equals(model.DateOfBirth))
                            .Where(x => model.PartnerId == null || x.PartnerId.Equals(model.PartnerId.Value))
                            .Where(x => model.VehicleId == null || x.VehicleId.Equals(model.VehicleId))
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsDriverViewModel())
                            .ToListAsync();
            foreach (DriverViewModel x in entity)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                x.LicensePlates = vehicle.LicensePlates;
                x.VehicleName = vehicle.Name;
                x.VehicleTypeLabel = (await _unitOfWork.VehicleTypeRepository.GetById(vehicle.VehicleTypeId)).Label;
            }
            return entity;

        }

        public async Task<Response> Update(Guid id, UpdateDriverModel model)
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

            entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId);
            entity.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(entity.DateOfBirth, model.DateOfBirth);
            entity.Gender = UpdateTypeOfNotNullAbleObject<bool>(entity.Gender, model.Gender);
            entity.LastName = UpdateTypeOfNullAbleObject<string>(entity.LastName, model.LastName);
            entity.FirstName = UpdateTypeOfNullAbleObject<string>(entity.FirstName, model.FirstName);
            // entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            entity.ModifiedDate = DateTime.Now;
            _unitOfWork.DriverRepository.Update(entity);
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
                                                            x => x.DriverId == id &&
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
    }
}