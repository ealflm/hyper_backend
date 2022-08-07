using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class DepositService : BaseService, IDepositService
    {
        public DepositService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<DepositViewModel> GetOrderId(DepositSearchModel model)
        {
            var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Equals("Nap tiền")).FirstOrDefaultAsync();
            var order = new Order()
            {
                OrderId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                CustomerId = model.CustomerId,
                TotalPrice = model.Amount,
                ServiceTypeId= serviceType.ServiceTypeId,
                Status = 1
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
                Status = 1
            };
            var uid = (await _unitOfWork.TransactionRepository.Query().OrderBy(x => x.Uid).LastOrDefaultAsync()).Uid + 1;
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
                    RequestUri = new Uri("https://currency-converter18.p.rapidapi.com/api/v1/convert?from=VND&to=USD&amount=" + model.Amount),
                    Headers =
                {
                    { "X-RapidAPI-Key", "f71b73fa25msh577d06701f5f1fap1160ddjsne5ea8a4de278" },
                    { "X-RapidAPI-Host", "currency-converter18.p.rapidapi.com" },
                },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var start = body.IndexOf("convertedAmount") + 17;
                    model.Amount = decimal.Parse(body.Substring(start, 4));
                }
                var username = "AXZb2SctDtdMVaazixC9p-3WKxyBW2evpzldMqYi3rFn8UwEwzW_SU7Q6-upjLFWaGgBn14xOAWCIB_t";
                var password = "EPOBnxf2gprkvwAGJSUInJa3TM0VSujkVNB7dY3ExgdJwT3bvl9syul1_JDl9p-jw1XUVaIXtqp3X7Zd";
                string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                               .GetBytes(username + ":" + password));
                var GrantType = new Dictionary<string, string>();
                GrantType.Add("grant_type", "client_credentials");
                request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://api-m.sandbox.paypal.com/v1/oauth2/token"),
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
                    RequestUri = new Uri("https://api-m.sandbox.paypal.com/v2/checkout/orders"),
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
                string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
                string partnerCode = "MOMODJMX20220717";
                string accessKey = "WehkypIRwPP14mHb";
                string serectkey = "3fq8h4CqAAPZcTTb3nCDpFKwEkQDsZzz";
                string orderInfo = "test";
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
                    { "partnerName", "Test" },
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
            if (model.resultCode == 0 || model.resultCode == 9000)
            {
                transaction.Status = 2;
                order.Status = 2;
                _unitOfWork.TransactionRepository.UpdateWithMultipleKey(transaction);
                _unitOfWork.OrderRepository.Update(order);
                var wallet = await _unitOfWork.WalletRepository.GetById(transaction.WalletId);
                wallet.AccountBalance += transaction.Amount;
                _unitOfWork.WalletRepository.Update(wallet);
                await _unitOfWork.SaveChangesAsync();
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

            var username = "AXZb2SctDtdMVaazixC9p-3WKxyBW2evpzldMqYi3rFn8UwEwzW_SU7Q6-upjLFWaGgBn14xOAWCIB_t";
            var password = "EPOBnxf2gprkvwAGJSUInJa3TM0VSujkVNB7dY3ExgdJwT3bvl9syul1_JDl9p-jw1XUVaIXtqp3X7Zd";
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(username + ":" + password));
            var GrantType = new Dictionary<string, string>();
            GrantType.Add("grant_type", "client_credentials");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api-m.sandbox.paypal.com/v1/oauth2/token"),
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
                RequestUri = new Uri("https://api.sandbox.paypal.com/v2/checkout/orders/" + id + "/capture"),
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
                    transaction.Status = 2;
                    order.Status = 2;
                    _unitOfWork.TransactionRepository.UpdateWithMultipleKey(transaction);
                    _unitOfWork.OrderRepository.Update(order);
                    var wallet = await _unitOfWork.WalletRepository.GetById(transaction.WalletId);
                    wallet.AccountBalance += transaction.Amount;
                    _unitOfWork.WalletRepository.Update(wallet);
                    await _unitOfWork.SaveChangesAsync();
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