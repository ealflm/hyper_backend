using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Authorization;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;
using TourismSmartTransportation.Data.Interfaces;
using AdminModel = TourismSmartTransportation.Data.Models.Admin;


namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class AuthorizationService : AccountService, IAuthorizationService
    {
        private readonly IConfiguration _configuration;

        public AuthorizationService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            _configuration = configuration;
        }

        public async Task<AuthorizationResultViewModel> Login(LoginSearchModel loginModel)
        {
            AuthorizationResultViewModel model = null;
            string result = null;
            string message= null;

            if (!string.IsNullOrEmpty(loginModel.Email) && !string.IsNullOrEmpty(loginModel.Password))
            {
                model = await LoginEmailPassword(loginModel.Email, loginModel.Password);
            }

            if (model != null && model.Data != null)
            {
                result = GetToken((AdminViewModel)model.Data);
            }

            message = model.Message;

            return new AuthorizationResultViewModel(result, message);
        }

        private async Task<AuthorizationResultViewModel> LoginEmailPassword(string email, string password)
        {
            AdminViewModel result = null;
            string message = null;
            var user = await _unitOfWork.AdminRepository.Query()
                .Where(x => x.Email == email && x.Password != null)
                .FirstOrDefaultAsync();

            if (user != null && VerifyPassword(password, user.Password, user.Salt))
            {
                if (user.Status == 1)
                {
                    result = new AdminViewModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        PhotoUrl = user.PhotoUrl,
                        Status = user.Status
                    };
                }
                else
                {
                    message = "The user doesn't have permission to access this resource";
                }
            }
            else
            {
                message = "Invalid user name or password";
            }

            return new AuthorizationResultViewModel(result, message);
        }

        private string GetToken(AdminViewModel model)
        {
            var authClaims = new List<Claim>
            {
                new Claim("Id", model.Id.ToString()),
                new Claim("FirstName", model.FirstName ?? ""),
                new Claim("LastName", model.LastName ?? ""),
                new Claim("Email", model.Email ?? ""),
                new Claim("PhoneNumber", model.PhoneNumber ?? ""),
                new Claim("Role", model.Status == 1 ? "admin" : "non-admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
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
                    PhoneNumber= model.PhoneNumber
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
