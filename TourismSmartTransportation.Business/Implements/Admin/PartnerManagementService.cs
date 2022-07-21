using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement;
using Vonage.Request;
using System.Net.Http;
using TourismSmartTransportation.Business.ViewModel.Admin.EmailManagement;
using TourismSmartTransportation.Business.SearchModel.Shared;
using Microsoft.AspNetCore.Http;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PartnerManagementService : AccountService, IPartnerManagementService
    {
        public PartnerManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, Credentials credentials,
                                        HttpClient client, ITwilioSettings twilioSettings) : base(unitOfWork, blobServiceClient, credentials, client, twilioSettings)
        {
        }

        private readonly string SUBJECT = "Thông Báo Tài Khoản Đăng Nhập";
        /// <summary>
        /// Create a new partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> AddPartner(AddPartnerModel model)
        {
            string username = GenerateUserNameAuto(model.FirstName, model.LastName);
            if (string.IsNullOrWhiteSpace(username))
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Thông tin không phù hợp!"
                };
            }
            int i = 0;
            while (i != -1)
            {
                string temp = i == 0 ? username : username + i;
                bool checkExistedUsername = await _unitOfWork.PartnerRepository
                                                .Query()
                                                .AnyAsync(x => x.Username.ToLower() == temp.ToLower());
                if (!checkExistedUsername)
                {
                    i = -1;
                    username = temp;
                }
                else
                {
                    ++i;
                }
            }
            string password = GeneratePasswordAuto(6);
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var partner = new TourismSmartTransportation.Data.Models.Partner()
            {
                PartnerId = Guid.NewGuid(),
                Username = username,
                Password = passwordHash,
                Salt = passwordSalt,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhotoUrl = await UploadFile(model.UploadFile, Container.Partner),
                Gender = model.Gender,
                Phone = model.Phone,
                Address1 = model.Address1,
                Address2 = model.Address2,
                CompanyName = model.CompanyName,
                DateOfBirth = model.DateOfBirth != null ? model.DateOfBirth.Value : null,
                Email = model.Email,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Status = 1
            };
            await _unitOfWork.PartnerRepository.Add(partner);
            if (model.ServiceTypeIdList != null)
            {
                foreach (Guid x in model.ServiceTypeIdList)
                {
                    var partnerService = new PartnerServiceType()
                    {
                        PartnerId = partner.PartnerId,
                        ServiceTypeId = x,
                        Status = 1
                    };
                    await _unitOfWork.PartnerServiceTypeRepository.Add(partnerService);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            if (partner.Email != null)
            {
                var email = new EmailViewModel()
                {
                    Subject = SUBJECT,
                    ToAddress = partner.Email,
                    Body = "Tên tài khoản: " + username + "\nMật khẩu: " + password
                };
                await SendEmail(email);
            }
            else
            {
                SendSMS(partner.Phone, "Ten tai khoan: " + username + "& Mat khau: " + password);
            }
            return new()
            {
                StatusCode = 201,
                Message = "Tạo tài khoản thành công!"
            };
        }

        /// <summary>
        /// Update status from active into inactive.
        /// User cannot do annything when account status is inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response> DeletePartner(Guid id)
        {

            var partner = await _unitOfWork.PartnerRepository.GetById(id);
            if (partner == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            partner.Status = 0;
            _unitOfWork.PartnerRepository.Update(partner);
            var serviceTypes = await _unitOfWork.PartnerServiceTypeRepository
                            .Query()
                            .Where(x => x.PartnerId.Equals(partner.PartnerId))
                            .ToListAsync();
            // foreach (PartnerServiceType x in serviceTypes)
            // {
            //     x.Status = 0;
            //     _unitOfWork.PartnerServiceTypeRepository.Update(x);
            // }

            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        /// <summary>
        /// Get details partner account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PartnerViewModel> GetPartner(Guid id)
        {
            var Partner = await _unitOfWork.PartnerRepository.GetById(id);
            if (Partner == null)
            {
                return null;
            }
            PartnerViewModel model = Partner.AsPartnerViewModel();
            model.DriverQuantity = _unitOfWork.DriverRepository.Query().Where(x => x.PartnerId.Equals(id)).Count();
            model.VehicleQuantity = _unitOfWork.VehicleRepository.Query().Where(x => x.PartnerId.Equals(id)).Count();
            var serviceTypes = await _unitOfWork.PartnerServiceTypeRepository.Query().Where(x => x.PartnerId.Equals(model.Id)).ToListAsync();
            model.ServiceTypeList = new List<ServiceTypeViewModel>();
            foreach (PartnerServiceType x in serviceTypes)
            {
                var serviecType = await _unitOfWork.ServiceTypeRepository.GetById(x.ServiceTypeId);
                model.ServiceTypeList.Add(serviecType.AsServiceTypeViewModel());
            }
            return model;
        }

        /// <summary>
        /// Search partner's accounts by Username and Status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SearchResultViewModel<PartnerViewModel>> SearchPartner(PartnerSearchModel model)
        {
            var companies = await _unitOfWork.PartnerRepository.Query()
                .Where(x => model.Username == null || x.Username.Contains(model.Username))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Username)
                .Select(x => x.AsPartnerViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(companies, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var c in listItemsAfterPaging)
            {
                var listServiceType = await _unitOfWork.PartnerServiceTypeRepository
                                    .Query()
                                    .Where(p => p.PartnerId == c.Id)
                                    .ToListAsync();
                c.ServiceTypeList = new List<ServiceTypeViewModel>();
                foreach (PartnerServiceType x in listServiceType)
                {
                    var serviecType = await _unitOfWork.ServiceTypeRepository.GetById(x.ServiceTypeId);
                    c.ServiceTypeList.Add(serviecType.AsServiceTypeViewModel());
                }
            }

            SearchResultViewModel<PartnerViewModel> result = null;
            result = new SearchResultViewModel<PartnerViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        /// <summary>
        /// Update information of partner's account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> UpdatePartner(Guid id, UpdatePartnerModel upm, List<Guid?> addList = null, List<Guid?> deleteList = null)
        {
            UpdatePartnerByAdmin model = new UpdatePartnerByAdmin()
            {
                // FirstName = upm.FirstName,
                // LastName = upm.LastName,
                Password = upm.Password,
                CompanyName = upm.CompanyName,
                Gender = upm.Gender,
                Phone = upm.Phone,
                Address1 = upm.Address1,
                Address2 = upm.Address2,
                Email = upm.Email,
                DateOfBirth = upm.DateOfBirth != null ? upm.DateOfBirth.Value : null,
                Status = upm.Status != null ? upm.Status.Value : null,
            };


            var partner = await _unitOfWork.PartnerRepository.GetById(id);
            if (partner == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            if (model.Password != null)
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                partner.Password = UpdateTypeOfNullAbleObject<byte[]>(partner.Password, passwordHash);
                partner.Salt = UpdateTypeOfNullAbleObject<byte[]>(partner.Salt, passwordSalt);
            }
            partner.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Partner, partner.PhotoUrl);
            partner.PhotoUrl += await UploadFile(model.UploadFile, Container.Partner);
            partner.FirstName = UpdateTypeOfNullAbleObject<string>(partner.FirstName, partner.FirstName);
            partner.LastName = UpdateTypeOfNullAbleObject<string>(partner.LastName, partner.LastName);
            partner.Address1 = UpdateTypeOfNullAbleObject<string>(partner.Address1, model.Address1);
            partner.Address2 = UpdateTypeOfNullAbleObject<string>(partner.Address2, model.Address2);
            partner.Phone = UpdateTypeOfNullAbleObject<string>(partner.Phone, model.Phone);
            partner.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(partner.DateOfBirth, model.DateOfBirth);
            partner.Email = UpdateTypeOfNullAbleObject<string>(partner.Email, model.Email);
            partner.CompanyName = UpdateTypeOfNullAbleObject<string>(partner.CompanyName, model.CompanyName);
            partner.Gender = UpdateTypeOfNotNullAbleObject<bool>(partner.Gender, model.Gender);
            partner.Status = UpdateTypeOfNotNullAbleObject<int>(partner.Status, model.Status);
            partner.ModifiedDate = DateTime.Now;
            if (deleteList != null)
            {
                model.DeleteServiceTypeIdList = deleteList.ToList();
                foreach (Guid p in model.DeleteServiceTypeIdList)
                {
                    var serviceType = await _unitOfWork.PartnerServiceTypeRepository
                                    .Query()
                                    .Where(x => x.PartnerId == partner.PartnerId && x.ServiceTypeId == p)
                                    .FirstOrDefaultAsync();
                    if (serviceType != null)
                    {
                        _unitOfWork.PartnerServiceTypeRepository.Remove(serviceType);
                    }
                }
            }
            var serviceTypes = await _unitOfWork.PartnerServiceTypeRepository.Query().Where(x => x.PartnerId.Equals(partner.PartnerId)).ToListAsync();

            if (addList != null)
            {
                model.AddServiceTypeIdList = addList.ToList();
                foreach (Guid x in model.AddServiceTypeIdList)
                {
                    var isExist = false;
                    foreach (PartnerServiceType y in serviceTypes)
                    {
                        if (x.Equals(y.ServiceTypeId))
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (isExist)
                    {
                        continue;
                    }
                    var serviceType = new PartnerServiceType()
                    {
                        PartnerId = partner.PartnerId,
                        ServiceTypeId = x,
                        Status = 1
                    };
                    await _unitOfWork.PartnerServiceTypeRepository.Add(serviceType);
                }
            }
            _unitOfWork.PartnerRepository.Update(partner);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        public async Task<Response> ChangePassowrd(PasswordVerificationModel model)
        {
            var partner = await _unitOfWork.PartnerRepository
                            .Query()
                            .Where(x => x.Username == model.Username)
                            .FirstOrDefaultAsync();
            if (partner == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Tài khoản không lệ!"
                };
            }

            if (!VerifyPassword(model.OldPassowrd, partner.Password, partner.Salt))
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Mặt khẩu cũ không chính xác!"
                };
            }

            // To keep some fields do not need to change
            partner.PhotoUrl = partner.PhotoUrl;
            partner.FirstName = UpdateTypeOfNullAbleObject<string>(partner.FirstName, null);
            partner.LastName = UpdateTypeOfNullAbleObject<string>(partner.LastName, null);
            partner.Address1 = UpdateTypeOfNullAbleObject<string>(partner.Address1, null);
            partner.Address2 = UpdateTypeOfNullAbleObject<string>(partner.Address2, null);
            partner.Phone = UpdateTypeOfNullAbleObject<string>(partner.Phone, null);
            partner.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(partner.DateOfBirth, null);
            partner.Email = UpdateTypeOfNullAbleObject<string>(partner.Email, null);
            partner.CompanyName = UpdateTypeOfNullAbleObject<string>(partner.CompanyName, null);
            partner.Gender = UpdateTypeOfNotNullAbleObject<bool>(partner.Gender, null);
            partner.Status = UpdateTypeOfNotNullAbleObject<int>(partner.Status, null);

            // Some fields will be change
            CreatePasswordHash(model.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            partner.Password = UpdateTypeOfNullAbleObject<byte[]>(partner.Password, passwordHash);
            partner.Salt = UpdateTypeOfNullAbleObject<byte[]>(partner.Salt, passwordSalt);
            partner.ModifiedDate = DateTime.Now;

            _unitOfWork.PartnerRepository.Update(partner);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Đổi mật khẩu thành công!"
            };
        }
    }
}