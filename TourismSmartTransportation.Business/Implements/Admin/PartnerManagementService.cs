﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PartnerManagementService : AccountService, IPartnerManagementService
    {
        public PartnerManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        /// <summary>
        /// Create a new partner account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddPartner(AddPartnerModel model)
        {
            bool isExist = await _unitOfWork.PartnerRepository.Query()
                .AnyAsync(x => x.Username == model.Username);
            if (isExist)
            {
                return false;
            }
            try
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var partner = new Partner()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CompanyName = model.CompanyName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Email = model.Email,
                    Password = passwordHash,
                    Salt = passwordSalt,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Username = model.Username,
                    PhotoUrl = UploadFile(model.UploadFile, Container.Partner).Result,
                    Status = 1
                };
                await _unitOfWork.PartnerRepository.Add(partner);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Update status from active into inactive.
        /// User cannot do annything when account status is inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeletePartner(Guid id)
        {
            try
            {
                var Partner = await _unitOfWork.PartnerRepository.GetById(id);
                Partner.Status = 0;
                _unitOfWork.PartnerRepository.Update(Partner);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
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
        public async Task<bool> UpdatePartner(Guid id, UpdatePartnerModel model)
        {
            try
            {
                var partner = await _unitOfWork.PartnerRepository.GetById(id);
                if (model.Password != null)
                {
                    CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    partner.Password = UpdateTypeOfNullAbleObject<byte[]>(partner.Password, passwordHash);
                    partner.Salt = UpdateTypeOfNullAbleObject<byte[]>(partner.Salt, passwordSalt);
                }
                partner.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Partner, partner.PhotoUrl);
                partner.PhotoUrl += await UploadFile(model.UploadFile, Container.Partner);
                partner.FirstName = UpdateTypeOfNullAbleObject<string>(partner.FirstName, model.FirstName);
                partner.LastName = UpdateTypeOfNullAbleObject<string>(partner.LastName, model.LastName);
                partner.Address1 = UpdateTypeOfNullAbleObject<string>(partner.Address1, model.Address1);
                partner.Address2 = UpdateTypeOfNullAbleObject<string>(partner.Address2, model.Address2);
                partner.Phone = UpdateTypeOfNullAbleObject<string>(partner.Phone, model.Phone);
                partner.DateOfBirth = UpdateTypeOfNotNullAbleObject<DateTime>(partner.DateOfBirth, model.DateOfBirth);
                partner.Email = UpdateTypeOfNullAbleObject<string>(partner.Email, model.Email);
                partner.CompanyName = UpdateTypeOfNullAbleObject<string>(partner.CompanyName, model.CompanyName);
                partner.Gender = UpdateTypeOfNotNullAbleObject<bool>(partner.Gender, model.Gender);
                partner.Status = UpdateTypeOfNotNullAbleObject<int>(partner.Status, model.Status);
                partner.ModifiedDate = DateTime.Now;
                _unitOfWork.PartnerRepository.Update(partner);
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