using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CompanyManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CompanyManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Business.Extensions;
using CompanyModel = TourismSmartTransportation.Data.Models.Company;
using Azure.Storage.Blobs;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class CompanyManagementService : AccountService, ICompanyManagementService
    {
        public CompanyManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<bool> AddCompany(AddCompanyViewModel model)
        {
            bool isExist = await _unitOfWork.CompanyRepository.Query()
                .AnyAsync(x => x.UserName == model.UserName);
            if (isExist)
            {
                return false;
            }
            try
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var company = new CompanyModel()
                {
                    Address = model.Address,
                    Name = model.Name,
                    Password = passwordHash,
                    Salt = passwordSalt,
                    UserName = model.UserName,
                    PhotoUrl = UploadFile(model.UploadFile, Container.Test).Result,
                    Status = 1
                };
                await _unitOfWork.CompanyRepository.Add(company);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteCompany(Guid id)
        {
            try
            {
                var company = await _unitOfWork.CompanyRepository.GetById(id);
                company.Status = 2;
                _unitOfWork.CompanyRepository.Update(company);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<CompanyViewModel> GetCompany(Guid id)
        {
            var company = await _unitOfWork.CompanyRepository.GetById(id);
            if (company == null)
            {
                return null;
            }

            CompanyViewModel model = company.AsCompanyViewModel();
            return model;
        }

        public async Task<SearchResultViewModel<CompanyViewModel>> SearchCompany(CompanySearchModel model)
        {
            var companies = await _unitOfWork.CompanyRepository.Query()
                .Where(x => model.Name == null || x.Name.Contains(model.Name))
                .Where(x => model.Address == null || x.Address.Contains(model.Address))
                .Where(x => model.UserName == null || x.UserName.Contains(model.UserName))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Name)
                .Select(x => x.AsCompanyViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(companies, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<CompanyViewModel> result = null;
            result = new SearchResultViewModel<CompanyViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems=totalRecord
            };
            return result;
        }

        public async Task<bool> UpdateCompany(Guid id, AddCompanyViewModel model)
        {
            try
            {
                var company = await _unitOfWork.CompanyRepository.GetById(id);
                if (model.Password != null)
                {
                    CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    company.Password = UpdateTypeOfNullAbleObject<byte[]>(company.Password, passwordHash);
                    company.Salt = UpdateTypeOfNullAbleObject<byte[]>(company.Salt, passwordSalt);
                }
                company.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Test, company.PhotoUrl);
                company.PhotoUrl += await UploadFile(model.UploadFile, Container.Test);
                company.Name = UpdateTypeOfNullAbleObject<string>(company.Name, model.Name);
                company.Address = UpdateTypeOfNullAbleObject<string>(company.Address, model.Address);
                company.UserName = UpdateTypeOfNullAbleObject<string>(company.UserName, model.UserName);
                company.Status = UpdateTypeOfNotNullAbleObject<int>(company.Status, model.Status);
                _unitOfWork.CompanyRepository.Update(company);
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