using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using CustomerModel = TourismSmartTransportation.Data.Models.Customer;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class CustomerManagementService : AccountService, ICustomerManagementService
    {
        public CustomerManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<bool> AddCustomer(AddCustomerViewModel model)
        {
            bool isExist = await _unitOfWork.CustomerRepository.Query()
                .AnyAsync(x => x.Email == model.Email);
            if (isExist)
            {
                return false;
            }
            try
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var customer = new CustomerModel()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Birthday= model.Birthday.Value,
                    Email= model.Email,
                    Gender= model.Gender.Value,
                    PhoneNumber= model.PhoneNumber,
                    Password = passwordHash,
                    Salt = passwordSalt,
                    PhotoUrl = UploadFile(model.UploadFile, Container.Test).Result,
                    Status = 1
                };
                await _unitOfWork.CustomerRepository.Add(customer);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetById(id);
                customer.Status = 2;
                _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<CustomerViewModel> GetCustomer(Guid id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(id);
            if (customer == null)
            {
                return null;
            }

            CustomerViewModel model = customer.AsCustomerViewModel();
            return model;
        }

        public async Task<SearchResultViewModel<CustomerViewModel>> SearchCustomer(CustomerSearchModel model)
        {
            var customers = await _unitOfWork.CustomerRepository.Query()
                .Where(x => model.FirstName == null || x.FirstName.Contains(model.FirstName))
                .Where(x => model.LastName == null || x.LastName.Contains(model.LastName))
                .Where(x => model.Email == null || x.Email.Contains(model.Email))
                .Where(x => model.PhoneNumber == null || x.PhoneNumber.Contains(model.PhoneNumber))
                .Where(x => model.Birthday == null || x.Birthday==model.Birthday.Value)
                .Where(x => model.Gender == null || x.Gender == model.Gender.Value)
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Email)
                .Select(x => x.AsCustomerViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(customers, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<CustomerViewModel> result = null;
            result = new SearchResultViewModel<CustomerViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<bool> UpdateCustomer(Guid id, AddCustomerViewModel model)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetById(id);
                if (model.Password != null)
                {
                    CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    customer.Password = UpdateTypeOfNullAbleObject<byte[]>(customer.Password, passwordHash);
                    customer.Salt = UpdateTypeOfNullAbleObject<byte[]>(customer.Salt, passwordSalt);
                }
                customer.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Test, customer.PhotoUrl);
                customer.PhotoUrl += await UploadFile(model.UploadFile, Container.Test);
                customer.FirstName = UpdateTypeOfNullAbleObject<string>(customer.FirstName, model.FirstName);
                customer.LastName = UpdateTypeOfNullAbleObject<string>(customer.LastName, model.LastName);
                customer.Email = UpdateTypeOfNullAbleObject<string>(customer.Email, model.Email);
                customer.PhoneNumber = UpdateTypeOfNullAbleObject<string>(customer.PhoneNumber, model.PhoneNumber);
                customer.Gender = UpdateTypeOfNotNullAbleObject<bool>(customer.Gender, model.Gender);
                customer.Birthday = UpdateTypeOfNotNullAbleObject<DateTime>(customer.Birthday, model.Birthday);
                customer.Status = UpdateTypeOfNotNullAbleObject<int>(customer.Status, model.Status);
                _unitOfWork.CustomerRepository.Update(customer);
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
