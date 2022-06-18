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
            var isExistCode = await _unitOfWork.VehicleRepository.Query().AnyAsync(x => x.LicensePlates.Equals(model.LicensePlates));
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Phương tiện đã tồn tại!"
                };
            }
            var price = new TourismSmartTransportation.Data.Models.Vehicle()
            {
                Id = Guid.NewGuid(),
                Color = model.Color,
                LicensePlates = model.LicensePlates,
                Name = model.Name,
                PartnerId = model.PartnerId,
                RentStationId = model.RentStationId,
                ServiceTypeId = model.ServiceTypeId,
                VehicleTypeId = model.VehicleTypeId,
                Status = 1
            };
            if (model.RentStationId != null)
            {
                var priceRenting = await _unitOfWork.PriceListOfRentingServiceRepository.Query().Where(x => x.CategoryId.Equals(model.CategoryId) && x.PublishYearId.Equals(model.PublishYearId)).FirstOrDefaultAsync();
                price.PriceRentingId = priceRenting.Id;
            }
            await _unitOfWork.VehicleRepository.Add(price);
            
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
            var isExistCode = await _unitOfWork.VehicleRepository.Query().AnyAsync(x => x.LicensePlates.Equals(model.LicensePlates));
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Phương tiện đã tồn tại!"
                };
            }
            var entity = await _unitOfWork.VehicleRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            if (entity.PriceRentingId != null)
            {
                var priceRenting = await _unitOfWork.PriceListOfRentingServiceRepository.GetById(entity.PriceRentingId.Value);
                if (priceRenting != null && (priceRenting.CategoryId != model.CategoryId || priceRenting.PublishYearId != model.PublishYearId))
                {
                    priceRenting.CategoryId = model.CategoryId != null ? model.CategoryId.Value : priceRenting.CategoryId;
                    priceRenting.PublishYearId = model.PublishYearId != null ? model.PublishYearId.Value : priceRenting.PublishYearId;
                    priceRenting = await _unitOfWork.PriceListOfRentingServiceRepository.Query().Where(x => x.CategoryId.Equals(priceRenting.CategoryId) && x.PublishYearId.Equals(priceRenting.PublishYearId)).FirstOrDefaultAsync();
                    entity.PriceRentingId = UpdateTypeOfNotNullAbleObject<Guid>(entity.PriceRentingId, priceRenting.Id);
                }
            }
            entity.RentStationId = UpdateTypeOfNotNullAbleObject<Guid>(entity.RentStationId, model.RentStationId);
            entity.ServiceTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.ServiceTypeId, model.ServiceTypeId);
            entity.VehicleTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.VehicleTypeId, model.VehicleTypeId);
            entity.Color = UpdateTypeOfNullAbleObject<string>(entity.Color, model.Color);
            entity.LicensePlates = UpdateTypeOfNullAbleObject<string>(entity.LicensePlates, model.LicensePlates);
            entity.Name = UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.VehicleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}