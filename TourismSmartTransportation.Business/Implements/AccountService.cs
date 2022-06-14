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
            phone ="84"+ phone.Substring(1);
            var VonageClient = new VonageClient(_credentials);
            var response = VonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
            {
                To = phone,
                From = "T3T",
                Text = message
            });
        }

        public async Task<HttpResponseMessage> SendEmail(EmailViewModel model)
        {
            var response = await _client.PostAsJsonAsync($"api/sendemail", model);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}