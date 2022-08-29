using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class DepositService : BaseService, IDepositService
    {
        private readonly string titleMoMo = "Nạp tiền từ MoMo";
        private readonly string titlePayPal = "Nạp tiền từ PayPal";
        private string mesMoMo = "";
        private string mesPayPal = "";
        private IFirebaseCloudMsgService _firebaseCloud;
        private INotificationCollectionService _notificationCollection;
        private IConfiguration _configuration;
        public DepositService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IFirebaseCloudMsgService firebaseCloud, INotificationCollectionService notificationCollection, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            _firebaseCloud = firebaseCloud;
            _notificationCollection = notificationCollection;
            _configuration = configuration;
        }

        public async Task<DepositViewModel> GetOrderId(DepositSearchModel model)
        {
            var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Equals(ServiceTypeDefaultData.DEPOSIT_SERVICE_NAME)).FirstOrDefaultAsync();
            var order = new Order()
            {
                OrderId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                CustomerId = model.CustomerId,
                TotalPrice = model.Amount,
                ServiceTypeId = serviceType.ServiceTypeId,
                Status = 3
            };
            await _unitOfWork.OrderRepository.Add(order);
            var walletId = (await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(model.CustomerId)).FirstOrDefaultAsync()).WalletId;
            var transaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                OrderId = order.OrderId,
                WalletId = walletId,
                Amount = order.TotalPrice,
                CreatedDate = DateTime.Now,
                Status = 2
            };
            int uid = 1;
            try
            {
                var uidTmp = (await _unitOfWork.TransactionRepository.Query().OrderBy(x => x.Uid).LastOrDefaultAsync()).Uid + 1;
            }
            catch
            {
                uid = 100000000;
            }
            //await _unitOfWork.SaveChangesAsync();
            //var transactionModel = await _unitOfWork.TransactionRepository.GetById(transaction.TransactionId);
            var id = "";
            var result = new DepositViewModel();
            if (model.Method == 0)
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_configuration["CurrencyConverter:Uri"] + model.Amount),
                    Headers =
                {
                    { "X-RapidAPI-Key", _configuration["CurrencyConverter:X-RapidAPI-Key"] },
                    { "X-RapidAPI-Host", _configuration["CurrencyConverter:X-RapidAPI-Host"] },
                },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var start = body.IndexOf("convertedAmount") + 17;
                    model.Amount = decimal.Parse(body.Substring(start, 4));
                }
                var username = _configuration["PayPal:username"];
                var password = _configuration["PayPal:password"];
                string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                               .GetBytes(username + ":" + password));
                var GrantType = new Dictionary<string, string>();
                GrantType.Add("grant_type", "client_credentials");
                request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_configuration["PayPal:Uri1"]),
                    Headers =
                    {
                        { "Authorization", "Basic " + encoded }
                    },
                    Content = new FormUrlEncodedContent(GrantType)
                };
                string token;
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var start = body.IndexOf("access_token") + 15;
                    int end = body.IndexOf("\"", start + 1);
                    token = body.Substring(start, end - start);
                }
                var Json = "{\"intent\": \"CAPTURE\",\"purchase_units\": [{\"amount\": {\"currency_code\": \"USD\",\"value\": \"" + model.Amount + "\"}}],\"application_context\": {\"return_url\": \"https://example.com/hyper?uid=" + uid + "&amount=" + transaction.Amount + "&create-date=" + new DateTimeOffset(transaction.CreatedDate).ToUnixTimeSeconds() + "\",\"cancel_url\": \"\"}}";
                request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_configuration["PayPal:Uri2"]),
                    Headers =
                    {
                        { "Authorization", "Bearer " + token}
                    },
                    Content = new StringContent(Json, Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var start = body.IndexOf("id") + 5;
                    int end = body.IndexOf("\"", start + 1);
                    id = body.Substring(start, end - start);
                }
                result.Id = id;
                transaction.Content = id;
            }
            else
            {
                string endpoint = _configuration["MoMo:uri"];
                string partnerCode = _configuration["MoMo:partnerCode"];
                string accessKey = _configuration["MoMo:accessKey"];
                string serectkey = _configuration["MoMo:serectkey"];
                string orderInfo = "Hyper";
                string redirectUrl = "hyper://customer.app?uid=" + uid + "&create-date=" + new DateTimeOffset(transaction.CreatedDate).ToUnixTimeSeconds();
                string ipnUrl = "https://tourism-smart-transportation-api.azurewebsites.net/api/v1.0/customer/deposit-momo"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
                string orderId = DateTime.Now.Ticks.ToString();
                string amount = model.Amount.ToString();
                string requestId = order.OrderId.ToString();
                string extraData = "";
                string requestType = "captureWallet";

                //Before sign HMAC SHA256 signature
                string rawHash = "accessKey=" + accessKey +
                    "&amount=" + amount +
                    "&extraData=" + extraData +
                    "&ipnUrl=" + ipnUrl +
                    "&orderId=" + orderId +
                    "&orderInfo=" + orderInfo +
                    "&partnerCode=" + partnerCode +
                    "&redirectUrl=" + redirectUrl +
                    "&requestId=" + requestId +
                    "&requestType=" + requestType;

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);

                //build body json request
                JObject message = new JObject
                {
                    { "partnerCode", partnerCode },
                    { "partnerName", "Hyper" },
                    { "storeId", "MomoTestStore" },
                    { "requestId", requestId },
                    { "amount", amount },
                    { "orderId", orderId },
                    { "orderInfo", orderInfo },
                    { "redirectUrl", redirectUrl },
                    { "ipnUrl", ipnUrl },
                    { "lang", "vi" },
                    { "extraData", extraData },
                    { "requestType", requestType },
                    { "signature", signature }

                };

                string responseFromMomo = await PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);

                result.Id = jmessage.GetValue("payUrl").ToString();
                transaction.Content = requestId;
                //result.Id = responseFromMomo;
            }
            await _unitOfWork.TransactionRepository.Add(transaction);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<Response> GetOrderMoMoStatus(MoMoStatusModel model)
        {
            var transaction = await _unitOfWork.TransactionRepository.Query().Where(x => x.OrderId.Equals(new Guid(model.requestId))).FirstOrDefaultAsync();
            var order = await _unitOfWork.OrderRepository.GetById(new Guid(model.requestId));
            var customer = await _unitOfWork.CustomerRepository.GetById(order.CustomerId);
            if (model.resultCode == 0 || model.resultCode == 9000)
            {
                transaction.Status = 1;
                transaction.Content = "Nạp tiền vào ví từ MoMo";
                order.Status = 2;
                _unitOfWork.TransactionRepository.UpdateWithMultipleKey(transaction);
                _unitOfWork.OrderRepository.Update(order);
                var wallet = await _unitOfWork.WalletRepository.GetById(transaction.WalletId);
                wallet.AccountBalance += transaction.Amount;
                _unitOfWork.WalletRepository.Update(wallet);
                await _unitOfWork.SaveChangesAsync();
                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                mesMoMo = string.Format(elGR, "Quý khách đã nạp thành công {0:N0} VNĐ từ MoMo", order.TotalPrice);
                await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, titleMoMo, mesMoMo);
                SaveNotificationModel noti = new SaveNotificationModel()
                {
                    CustomerId = customer.CustomerId.ToString(),
                    CustomerFirstName = customer.FirstName,
                    CustomerLastName = customer.LastName,
                    Title = titleMoMo,
                    Message = mesMoMo,
                    Type = "Deposit",
                    Status = (int)NotificationStatus.Active
                };
                await _notificationCollection.SaveNotification(noti);
                return new()
                {
                    StatusCode = 204,
                    Message = "Thanh toán thành công!"
                };
            }
            transaction.Status = 0;
            order.Status = 0;
            _unitOfWork.TransactionRepository.UpdateWithMultipleKey(transaction);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 400,
                Message = "Thanh toán thất bại!"
            };

        }

        public async Task<Response> GetOrderStatus(string id)
        {
            var transaction = await _unitOfWork.TransactionRepository.Query().Where(x => x.Content.Equals(id)).FirstOrDefaultAsync();
            var order = await _unitOfWork.OrderRepository.GetById(transaction.OrderId);
            var client = new HttpClient();

            var username = _configuration["PayPal:username"];
            var password = _configuration["PayPal:password"];
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(username + ":" + password));
            var GrantType = new Dictionary<string, string>();
            GrantType.Add("grant_type", "client_credentials");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_configuration["PayPal:Uri1"]),
                Headers =
                    {
                        { "Authorization", "Basic " + encoded }
                    },
                Content = new FormUrlEncodedContent(GrantType)
            };
            string token;
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var start = body.IndexOf("access_token") + 15;
                int end = body.IndexOf("\"", start + 1);
                token = body.Substring(start, end - start);
            }
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_configuration["PayPal:Uri2"] + id + "/capture"),
                Headers =
                    {
                        { "Authorization", "Bearer " + token }
                    },
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            using (var response = await client.SendAsync(request))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    transaction.Status = 1;
                    transaction.Content = "Nạp tiền vào ví từ Paypal";
                    order.Status = 2;
                    _unitOfWork.TransactionRepository.UpdateWithMultipleKey(transaction);
                    _unitOfWork.OrderRepository.Update(order);
                    var wallet = await _unitOfWork.WalletRepository.GetById(transaction.WalletId);
                    wallet.AccountBalance += transaction.Amount;
                    _unitOfWork.WalletRepository.Update(wallet);
                    await _unitOfWork.SaveChangesAsync();
                    var customer = await _unitOfWork.CustomerRepository.GetById(order.CustomerId);
                    CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                    mesPayPal = string.Format(elGR, "Quý khách đã nạp thành công {0:N0} VNĐ từ PayPal", order.TotalPrice);
                    await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, titlePayPal, mesPayPal);
                    SaveNotificationModel noti = new SaveNotificationModel()
                    {
                        CustomerId = customer.CustomerId.ToString(),
                        CustomerFirstName = customer.FirstName,
                        CustomerLastName = customer.LastName,
                        Title = titlePayPal,
                        Message = mesPayPal,
                        Type = "Deposit",
                        Status = (int)NotificationStatus.Active
                    };
                    await _notificationCollection.SaveNotification(noti);
                    return new()
                    {
                        StatusCode = 200,
                        Message = "Thanh toán thành công!"
                    };
                }
            }
            transaction.Status = 0;
            order.Status = 0;
            _unitOfWork.TransactionRepository.UpdateWithMultipleKey(transaction);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 400,
                Message = "Thanh toán thất bại!"
            };
        }


    }
}