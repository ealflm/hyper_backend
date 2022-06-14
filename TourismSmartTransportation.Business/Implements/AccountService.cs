using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Interfaces;
using Vonage;
using Vonage.Request;

namespace TourismSmartTransportation.Business.Implements
{
    public class AccountService : BaseService
    {
        private readonly Credentials _credentials;
        public AccountService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public AccountService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, Credentials credentials) : base(unitOfWork, blobServiceClient)
        {
            _credentials = credentials;
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

    }
}