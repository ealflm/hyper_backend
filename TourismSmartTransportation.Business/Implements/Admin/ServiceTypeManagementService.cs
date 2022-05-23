using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.ServiceType;
using TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class ServiceTypeManagementService : BaseService, IServiceTypeManagementService
    {
        public ServiceTypeManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<bool> Create(ServiceTypeSearchModel model)
        {
            bool isExist = await _unitOfWork.ServiceTypeRepository.Query()
                .AnyAsync(x => x.Name == model.Name);
            if (isExist)
            {
                return false;
            }
            try
            {
                var serviceType = model.AsServiceTypeDataModel();
                serviceType.Status = 1;
                serviceType.CreatedDate = DateTime.Now;
                serviceType.ModifiedDate = DateTime.Now;
                await _unitOfWork.ServiceTypeRepository.Add(serviceType);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(id);
                serviceType.Status = 0;
                _unitOfWork.ServiceTypeRepository.Update(serviceType);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<ServiceTypeViewModel> Get(Guid id)
        {
            var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(id);
            if (serviceType == null)
            {
                return null;
            }

            var model = serviceType.AsServiceTypeViewModel();
            return model;
        }

        public async Task<SearchResultViewModel<ServiceTypeViewModel>> GetAll()
        {
            var serviceTypeList = await _unitOfWork.ServiceTypeRepository.Query()
                 .Select(x => x.AsServiceTypeViewModel())
                 .ToListAsync();
            SearchResultViewModel<ServiceTypeViewModel> result = null;
            result = new SearchResultViewModel<ServiceTypeViewModel>()
            {
                Items = serviceTypeList,
                PageSize = 1,
                TotalItems = serviceTypeList.Count
            };
            return result;
        }

        public async Task<bool> Update(Guid id, ServiceTypeSearchModel model)
        {
            try
            {

                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(id);
                serviceType.Name = UpdateTypeOfNullAbleObject(serviceType.Name, model.Name);
                serviceType.Content = UpdateTypeOfNullAbleObject(serviceType.Content, model.Content);
                serviceType.ModifiedDate = DateTime.Now;
                serviceType.Status = UpdateTypeOfNotNullAbleObject<int>(serviceType.Status, model.Status);
                _unitOfWork.ServiceTypeRepository.Update(serviceType);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
