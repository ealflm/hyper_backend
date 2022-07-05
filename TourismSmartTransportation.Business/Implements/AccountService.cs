using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Admin.EmailManagement;
using TourismSmartTransportation.Data.Interfaces;
using Vonage;
using Vonage.Request;
using Vonage.Verify;

namespace TourismSmartTransportation.Business.Implements
{
    public class AccountService : BaseService
    {
        private readonly Credentials _credentials;
        private readonly HttpClient _client;
        public AccountService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public AccountService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, Credentials credentials, HttpClient client) : base(unitOfWork, blobServiceClient)
        {
            _credentials = credentials;
            _client = client;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public void SendSMS(string phone, string message)
        {
            switch (phone)
            {
                case "0378980164":
                    {
                        var credentials = Credentials.FromApiKeyAndSecret("e9878e1f", "rddwLbyDS1f530HV");
                        var client = new VonageClient(credentials);
                        var response = client.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
                        {
                            To = "84378980164",
                            From = "T3T",
                            Text = message
                        });
                        break;
                    }
                default:
                    {
                        phone = "84" + phone.Substring(1);
                        var VonageClient = new VonageClient(_credentials);
                        var response = VonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
                        {
                            To = phone,
                            From = "T3T",
                            Text = message
                        });
                        break;
                    }
            }
        }

        public async Task<HttpResponseMessage> SendEmail(EmailViewModel model)
        {
            var response = await _client.PostAsJsonAsync($"api/sendemail", model);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<string> SendOTPVerification(string phone)
        {
            string requestId = "";
            switch (phone)
            {
                case "0378980164":
                    {
                        var credentials = Credentials.FromApiKeyAndSecret("e9878e1f", "rddwLbyDS1f530HV");
                        var client = new VonageClient(credentials);
                        var request = new VerifyRequest()
                        {
                            Brand = "T3T",
                            Number = "84378980164",
                        };
                        var response = await client.VerifyClient.VerifyRequestAsync(request);
                        Console.WriteLine($"Verify Request Complete\nStatus:{response.Status}\nRequest ID:{response.RequestId} {request}");
                        requestId = response.RequestId;
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Số điện thoại chưa được đăng ký");
                        break;
                    }
            }
            return requestId;
        }

        public async Task<int> VerifyCheckOTP(string phone, string OtpCode, string requestId)
        {
            int statusCode = 500;
            switch (phone)
            {
                case "0378980164":
                    {
                        var credentials = Credentials.FromApiKeyAndSecret("e9878e1f", "rddwLbyDS1f530HV");
                        var client = new VonageClient(credentials);
                        var request = new VerifyCheckRequest() { Code = OtpCode, RequestId = requestId };
                        var response = await client.VerifyClient.VerifyCheckAsync(request);
                        statusCode = int.Parse(response.Status);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Số điện thoại chưa được đăng ký");
                        break;
                    }
            }
            return statusCode;
        }
    }
}