﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Authorization;
using TourismSmartTransportation.Business.SearchModel.Common.Authorization;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer.Authorization;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;

namespace TourismSmartTransportation.Business.Interfaces
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResultViewModel> Login(LoginSearchModel model, Login loginType);
        Task<AuthorizationResultViewModel> RegisterForAdmin(RegisterSearchModel model);
        Task<Response> RegisterForCustomer(RegisterModel model);
        Task<Response> CheckExistedPhoneNumber(string phoneNumber, User role = User.Customer);
        Task<Response> SendOTP(string phone);
        Task<Response> VerifyOTP(OTPVerificationModel model);
        Task<Response> SendOTPByTwilio(string phone);
        Task<Response> VerifyOTPByTwilio(OTPVerificationModel model);
        Task<Response> ChangePasswordAllRole(PasswordVerificationModel model, Login role);
    }
}

