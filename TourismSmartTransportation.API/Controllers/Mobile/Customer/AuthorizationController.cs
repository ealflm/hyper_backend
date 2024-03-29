﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Utilities.Response;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Common.Authorization;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer.Authorization;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    public class AuthorizationController : BaseController
    {
        private readonly Business.Interfaces.IAuthorizationService _authorizationService;
        private readonly ITwilioSettings _twilioSettings;

        public AuthorizationController(Business.Interfaces.IAuthorizationService authorizationService, ITwilioSettings twilioSettings)
        {
            this._authorizationService = authorizationService;
            _twilioSettings = twilioSettings;
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Login)]
        public async Task<IActionResult> Login([FromBody] LoginSearchModel model)
        {
            var result = await _authorizationService.Login(model, global::Login.Customer);

            if (result.Data == null)
            {
                return Problem(result.Message, "Unauthorization", 401);
            }
            var loginViewModel = new LoginViewModel(result.Data.ToString());
            return Ok(loginViewModel);
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Register)]
        public async Task<IActionResult> RegisterForCustomer([FromForm] RegisterModel model)
        {
            return SendResponse(await _authorizationService.RegisterForCustomer(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.CheckPhoneNumber)]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CheckPhoneNumber(string phoneNumber)
        {
            return SendResponse(await _authorizationService.CheckExistedPhoneNumber(phoneNumber));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.OTP + "/send-otp")]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> SendOTP([FromForm] string phoneNumber)
        {
            return ResponseOTP(await _authorizationService.SendOTPByTwilio(phoneNumber));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.OTP + "/verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromForm] OTPVerificationModel model)
        {
            return SendResponse(await _authorizationService.VerifyOTPByTwilio(model));
        }

        [HttpPut]
        [Route(ApiVer1Url.Customer.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromForm] PasswordVerificationModel model)
        {
            return SendResponse(await _authorizationService.ChangePasswordAllRole(model, global::Login.Customer));
        }

    }
}
