using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
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
        public CustomerManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, ITwilioSettings twilioSettings) : base(unitOfWork, blobServiceClient, twilioSettings)
        {
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> AddCustomer(AddCustomerModel model)
        {
            bool isExist = await _unitOfWork.CustomerRepository.Query()
                .AnyAsync(x => x.Phone == model.Phone);
            if (isExist)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Số điện thoại đã được sử dụng!"
                };
            }

            CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var customer = new CustomerModel()
            {
                CustomerId = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = passwordHash,
                Salt = passwordSalt,
                Gender = model.Gender,
                Phone = model.Phone,
                Address1 = model.Address1,
                Address2 = model.Address2,
                PhotoUrl = UploadFile(model.UploadFile, Container.Customer).Result,
                DateOfBirth = model.Birthday != null ? model.Birthday.Value : null,
                Email = model.Email,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Status = 1
            };
            await _unitOfWork.CustomerRepository.Add(customer);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo tài khoản thành công!"
            };
        }

        /// <summary>
        /// Update status from active to inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response> DeleteCustomer(Guid id)
        {

            var customer = await _unitOfWork.CustomerRepository.GetById(id);
            if (customer == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy!"
                };
            }
            customer.Status = 0;
            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
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
            var hasCard = await _unitOfWork.CardRepository.Query().AnyAsync(x => x.CustomerId.Equals(model.Id));
            if (hasCard)
            {
                model.CardUid = (await _unitOfWork.CardRepository.Query().Where(x => x.CustomerId.Equals(model.Id)).FirstOrDefaultAsync()).Uid;
            }
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
                .Where(x => model.Phone == null || x.Phone.Contains(model.Phone))
                .Where(x => model.LastName == null || x.LastName.Contains(model.LastName))
                .Where(x => model.FirstName == null || x.FirstName.Contains(model.FirstName))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.FirstName)
                .Select(x => x.AsCustomerViewModel())
                .ToListAsync();
            foreach (CustomerViewModel x in customers)
            {
                var hasCard = await _unitOfWork.CardRepository.Query().AnyAsync(y => y.CustomerId.Equals(x.Id));
                if (hasCard)
                {
                    x.CardUid = (await _unitOfWork.CardRepository.Query().Where(y => y.CustomerId.Equals(x.Id)).FirstOrDefaultAsync()).Uid;
                }
            }
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
        public async Task<Response> UpdateCustomer(Guid id, UpdateCustomerModel model)
        {
            if (!string.IsNullOrEmpty(model.Phone))
            {
                var isExist = await _unitOfWork.CustomerRepository.Query().AnyAsync(x => x.Phone == model.Phone);
                if (isExist)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Số điện thoại đã được sử dụng!"
                    };
                }
            }

            var customer = await _unitOfWork.CustomerRepository.GetById(id);
            if (model.Password != null)
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                customer.Password = UpdateTypeOfNullAbleObject<byte[]>(customer.Password, passwordHash);
                customer.Salt = UpdateTypeOfNullAbleObject<byte[]>(customer.Salt, passwordSalt);
            }
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

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}
