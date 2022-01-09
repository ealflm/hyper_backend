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
using CompanyModel = TourismSmartTransportation.Data.Models.Company;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class CompanyManagementService : AccountService, ICompanyManagementService
    {
        public CompanyManagementService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
                    PhotoUrl = model.PhotoUrl,
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
            CompanyViewModel model = new CompanyViewModel()
            {
                Id = company.Id,
                Address = company.Address,
                Name = company.Name,
                UserName = company.UserName,
                PhotoUrl = company.PhotoUrl,
                Status = company.Status
            };
            return model;
        }

        public async Task<SearchResultViewModel> SearchCompany(CompanySearchModel model)
        {
            int companyCount = _unitOfWork.CompanyRepository.Query().Count();
            var companies = await _unitOfWork.CompanyRepository.Query()
                .Where(x => model.Name == null || x.Name.Contains(model.Name))
                .Where(x => model.Address == null || x.Address.Contains(model.Address))
                .Where(x => model.UserName == null || x.UserName.Contains(model.UserName))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Name)
                .Skip(model.ItemsPerPage * Math.Min(model.PageIndex - 1, 0))
                .Take(model.ItemsPerPage > 0 ? model.ItemsPerPage : companyCount)
                .Select(x => new CompanyViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    UserName = x.UserName,
                    PhotoUrl = x.PhotoUrl,
                    Status = x.Status
                })
                .ToListAsync();
            SearchResultViewModel result = null;
            if (companies.Count > 0)
            {
                result = new SearchResultViewModel()
                {
                    Items = companies.ToList<object>(),
                    PageSize = model.ItemsPerPage == 0 ? 1 : ((companyCount / model.ItemsPerPage) + (companyCount % model.ItemsPerPage > 0 ? 1 : 0))
                };
            }
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
                company.Name = UpdateTypeOfNullAbleObject<string>(company.Name, model.Name);
                company.Address = UpdateTypeOfNullAbleObject<string>(company.Address, model.Address);
                company.UserName = UpdateTypeOfNullAbleObject<string>(company.UserName, model.UserName);
                company.PhotoUrl = UpdateTypeOfNullAbleObject<string>(company.PhotoUrl, model.PhotoUrl);
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