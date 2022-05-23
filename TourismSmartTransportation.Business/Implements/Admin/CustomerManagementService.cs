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

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddCustomer(AddCustomerModel model)
        {
            bool isExist = await _unitOfWork.CustomerRepository.Query()
                .AnyAsync(x => x.Phone == model.Phone);
            if (isExist)
            {
                return false;
            }
            try
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var customer = new CustomerModel()
                {
                    TierId = model.TierId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.Birthday != null ? model.Birthday.Value : null,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    Email = model.Email,
                    Gender = model.Gender,
                    Phone = model.Phone,
                    Password = passwordHash,
                    Salt = passwordSalt,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    PhotoUrl = UploadFile(model.UploadFile, Container.Customer).Result,
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

        /// <summary>
        /// Update status from active to inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCustomer(Guid id)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetById(id);
                customer.Status = 0;
                _unitOfWork.CustomerRepository.Update(customer);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get details customer's information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Search customers by requirement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SearchResultViewModel<CustomerViewModel>> SearchCustomer(CustomerSearchModel model)
        {
            var customers = await _unitOfWork.CustomerRepository.Query()
                .Where(x => model.TierId == null || x.Phone.Equals(model.TierId))
                .Where(x => model.Phone == null || x.Phone.Equals(model.Phone))
                .Where(x => model.LastName == null || x.LastName.Contains(model.LastName))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.LastName)
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

        /// <summary>
        /// Update customer's information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateCustomer(Guid id, UpdateCustomerModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    var isExist = await _unitOfWork.CustomerRepository.Query().AnyAsync(x => x.Phone == model.Phone);
                    if (isExist)
                    {
                        return false;
                    }
                }

                var customer = await _unitOfWork.CustomerRepository.GetById(id);
                if (model.Password != null)
                {
                    CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    customer.Password = UpdateTypeOfNullAbleObject<byte[]>(customer.Password, passwordHash);
                    customer.Salt = UpdateTypeOfNullAbleObject<byte[]>(customer.Salt, passwordSalt);
                }
                customer.TierId = model.TierId; // Customer cannot need tier, so we always need add value tier when updating data.
                customer.Phone = UpdateTypeOfNullAbleObject<string>(customer.Phone, model.Phone);
                customer.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Customer, customer.PhotoUrl);
                customer.PhotoUrl += await UploadFile(model.UploadFile, Container.Customer);
                customer.FirstName = UpdateTypeOfNullAbleObject<string>(customer.FirstName, model.FirstName);
                customer.LastName = UpdateTypeOfNullAbleObject<string>(customer.LastName, model.LastName);
                customer.Address1 = UpdateTypeOfNullAbleObject<string>(customer.Address1, model.Address1);
                customer.Address2 = UpdateTypeOfNullAbleObject<string>(customer.Address2, model.Address2);
                customer.Email = UpdateTypeOfNullAbleObject<string>(customer.Email, model.Email);
                customer.Gender = UpdateTypeOfNotNullAbleObject<bool>(customer.Gender, model.Gender);
                customer.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(customer.DateOfBirth, model.Birthday);
                customer.Status = UpdateTypeOfNotNullAbleObject<int>(customer.Status, model.Status);
                customer.ModifiedDate = DateTime.Now;
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
