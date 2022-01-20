﻿using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.SearchModel.Admin.Authorization;
using TourismSmartTransportation.Business.SearchModel.Common.Authorization;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;
using TourismSmartTransportation.Business.ViewModel.Company.Authorization;
using TourismSmartTransportation.Data.Interfaces;
using AdminModel = TourismSmartTransportation.Data.Models.Admin;


namespace TourismSmartTransportation.Business.Implements
{
    public class AuthorizationService : AccountService, IAuthorizationService
    {
        private readonly IConfiguration _configuration;

        public AuthorizationService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            _configuration = configuration;
        }

        public async Task<AuthorizationResultViewModel> Login(LoginSearchModel loginModel, Login loginType)
        {
            AuthorizationResultViewModel model = null;
            string result = null;
            string message= null;

            if (!string.IsNullOrEmpty(loginModel.UserName) && !string.IsNullOrEmpty(loginModel.Password))
            {
                switch ((int)loginType)
                {
                    case 0:
                        {
                            model = await LoginEmailPassword<AdminViewModel>(loginModel.UserName, loginModel.Password, new AdminViewModel(), loginType);
                            break;
                        }
                    case 1:
                        {
                            model = await LoginEmailPassword<CompanyViewModel>(loginModel.UserName, loginModel.Password, new CompanyViewModel(), loginType);
                            break;
                        }
                    case 2:
                        {
                            model = await LoginEmailPassword<AdminViewModel>(loginModel.UserName, loginModel.Password, new AdminViewModel(), loginType);
                            break;
                        }
                    case 3:
                        {
                            model = await LoginEmailPassword<AdminViewModel>(loginModel.UserName, loginModel.Password, new AdminViewModel(), loginType);
                            break;
                        }
                }
                
            }

            if (model != null && model.Data != null)
            {
                switch ((int)loginType)
                {
                    case 0:
                        {
                            result = GetToken((AdminViewModel)model.Data);
                            break;
                        }
                    case 1:
                        {
                            result = GetToken((CompanyViewModel)model.Data);
                            break;
                        }
                    case 2:
                        {
                            result = GetToken((AdminViewModel)model.Data);
                            break;
                        }
                    case 3:
                        {
                            result = GetToken((AdminViewModel)model.Data);
                            break;
                        }
                }
                
            }

            message = model.Message;

            return new AuthorizationResultViewModel(result, message);
        }

        private async Task<AuthorizationResultViewModel> LoginEmailPassword<T>(string email, string password, T result, Login loginType) where T : class
        {
            string message = null;
            dynamic user = null;

            switch ((int)loginType)
            {
                case 0:
                    {
                        user = await _unitOfWork.AdminRepository.Query()
                        .Where(x => x.Email == email && x.Password != null)
                        .FirstOrDefaultAsync();
                        break;
                    }
                case 1:
                    {
                        user = await _unitOfWork.CompanyRepository.Query()
                        .Where(x => x.UserName == email && x.Password != null)
                        .FirstOrDefaultAsync();
                        break;
                    }
                case 2:
                    {
                        user = await _unitOfWork.DriverRepository.Query()
                        .Where(x => x.PhoneNumber == email && x.Password != null)
                        .FirstOrDefaultAsync();
                        break;
                    }
                case 3:
                    {
                        user = await _unitOfWork.CustomerRepository.Query()
                        .Where(x => x.PhoneNumber == email && x.Password != null)
                        .FirstOrDefaultAsync();
                        break;
                    }
            }
            

             
            if (user != null && VerifyPassword(password, user.Password, user.Salt))
            {
                if (user.Status == 1)
                {
                    foreach(var x in result.GetType().GetProperties())
                    {
                        foreach(var y in user.GetType().GetProperties())
                        {
                            if (x.Name.Equals(y.Name))
                            {
                                x.SetValue(result, y.GetValue(user));
                            }
                        }
                    }
                }
                else
                {
                    message = "The user doesn't have permission to access this resource";
                    result = null;
                }
            }
            else
            {
                message = "Invalid user name or password";
                result = null;
            }

            return new AuthorizationResultViewModel(result, message);
        }

        private string GetToken<T>(T model) where T: class
        {
            var authClaims = new List<Claim>();
            foreach(var x in model.GetType().GetProperties())
            {
                authClaims.Add(new Claim(x.Name, (x.GetValue(model)?? "").ToString()));
            }
            authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }

        public async Task<AuthorizationResultViewModel> Register(RegisterSearchModel model)
        {
            AuthorizationResultViewModel resultViewModel = null;
            bool isExist = await _unitOfWork.AdminRepository.Query()
                .AnyAsync(x => x.Email == model.Email);
            if (!isExist)
            {
                CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var admin = new AdminModel
                {
                    FirstName= model.FirstName,
                    LastName= model.LastName,
                    Email = model.Email,
                    Password = passwordHash,
                    Salt = passwordSalt,
                    PhoneNumber= model.PhoneNumber,
                    PhotoUrl= await UploadFile(model.UploadFile, Container.Admin)
                };
                admin.Status = 1;

                await _unitOfWork.AdminRepository.Add(admin);
                await _unitOfWork.SaveChangesAsync();
  
            }
            else
            {
                resultViewModel = new AuthorizationResultViewModel(null, "This email address has already been registered");
            }

            return resultViewModel;
        }

        
    }
}