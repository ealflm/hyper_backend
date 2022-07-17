﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
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
            var order = new Order()
            {
                OrderId = Guid.NewGuid(),
                CreatedDate= DateTime.Now,
                CustomerId= model.CustomerId,
                TotalPrice= model.Amount,
                Status= 1
            };
            await _unitOfWork.OrderRepository.Add(order);
            var walletId = (await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(model.CustomerId)).FirstOrDefaultAsync()).WalletId;
            var transaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                OrderId = order.OrderId,
                WalletId = walletId,
                Amount = order.TotalPrice,
                CreatedDate = DateTime.Now
            };
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
                    var start= body.IndexOf("convertedAmount") + 17;
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
                var Json = "{\"intent\": \"CAPTURE\",\"purchase_units\": [{\"amount\": {\"currency_code\": \"USD\",\"value\": \""+ model.Amount+ "\"}}],\"application_context\": {\"return_url\": \"https://tourism-smart-transportation-api.azurewebsites.net/api/v1.0/customer/deposit/"+transaction.TransactionId+"\",\"cancel_url\": \"\"}}";
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
                await _unitOfWork.TransactionRepository.Add(transaction);
            }
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<Response> GetOrderStatus(Guid id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetById(id);
            var order = await _unitOfWork.OrderRepository.GetById(transaction.OrderId);
            var status = "";
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
                RequestUri = new Uri("https://api.sandbox.paypal.com/v2/checkout/orders/"+transaction.Content+ "/capture"),
                Headers =
                    {
                        { "Authorization", "Bearer " + token }
                    },
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };
            using (var response = await client.SendAsync(request))
            {
                if (response.StatusCode== System.Net.HttpStatusCode.Created)
                {
                    transaction.Status = 2;
                    order.Status = 2;
                    _unitOfWork.TransactionRepository.Update(transaction);
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
            _unitOfWork.TransactionRepository.Update(transaction);
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