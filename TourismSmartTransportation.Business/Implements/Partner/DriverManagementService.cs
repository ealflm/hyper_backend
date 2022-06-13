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

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class DriverManagementService : AccountService, IDriverManagementService
    {
        public DriverManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
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
            CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var driver = new Driver()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                DateOfBirth= model.DateOfBirth,
                FirstName= model.FirstName,
                Gender= model.Gender,
                LastName = model.LastName,
                ModifiedDate= DateTime.Now,
                PartnerId= model.PartnerId,
                Phone = model.Phone,
                VehicleId= model.VehicleId,
                Password= passwordHash,
                Salt= passwordSalt,
                Status = 1
            };

            await _unitOfWork.DriverRepository.Add(driver);
            await _unitOfWork.SaveChangesAsync();

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
            model.LicensePlates = (await _unitOfWork.VehicleRepository.GetById(model.VehicleId)).LicensePlates;
            return model;

        }

        public async Task<List<DriverViewModel>> Search(DriverSearchModel model)
        {
            var entity = await _unitOfWork.DriverRepository.Query()
                            .Where(x => model.FirstName == null || x.FirstName.Equals(model.FirstName))
                            .Where(x => model.LastName == null || x.LastName.Equals(model.LastName))
                            .Where(x => model.Phone == null || x.Phone.Equals(model.Phone))
                            .Where(x => model.DateOfBirth == null || x.DateOfBirth.Equals(model.DateOfBirth))
                            .Where(x => model.PartnerId == null || x.PartnerId.Equals(model.PartnerId.Value))
                            .Where(x => model.VehicleId == null || x.VehicleId.Equals(model.VehicleId))
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsDriverViewModel())
                            .ToListAsync();
            foreach(DriverViewModel x in entity)
            {
                x.LicensePlates = (await _unitOfWork.VehicleRepository.GetById(x.VehicleId)).LicensePlates;
            }
            return entity;

        }

        public async Task<Response> Update(Guid id, UpdateDriverModel model)
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
            var entity = await _unitOfWork.DriverRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.VehicleId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleId, model.VehicleId);
            entity.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(entity.DateOfBirth, model.DateOfBirth);
            entity.Gender = UpdateTypeOfNotNullAbleObject<bool>(entity.Gender, model.Gender);
            entity.LastName = UpdateTypeOfNullAbleObject<string>(entity.LastName, model.LastName);
            entity.FirstName = UpdateTypeOfNullAbleObject<string>(entity.FirstName, model.FirstName);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            entity.ModifiedDate = DateTime.Now;
            _unitOfWork.DriverRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}