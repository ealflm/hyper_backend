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
                    Password = passwordHash,
                    Salt = passwordSalt,
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

        public async Task<bool> DeletePartner(Guid id)
        {
            try
            {
                var Partner = await _unitOfWork.PartnerRepository.GetById(id);
                Partner.Status = 2;
                _unitOfWork.PartnerRepository.Update(Partner);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

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

        public async Task<bool> UpdatePartner(Guid id, AddPartnerModel model)
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
                partner.Gender = UpdateTypeOfNotNullAbleObject<bool>(partner.Gender, model.Gender);
                partner.Status = UpdateTypeOfNotNullAbleObject<int>(partner.Status, model.Status);
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