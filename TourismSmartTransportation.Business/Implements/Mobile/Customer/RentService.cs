using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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
    public class RentService : BaseService, IRentService
    {
        private IFirebaseCloudMsgService _firebaseCloud;
        private INotificationCollectionService _notificationCollection;
        private static IConfiguration _configuration;
        public RentService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IFirebaseCloudMsgService firebaseCloud, INotificationCollectionService notificationCollection, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            _firebaseCloud = firebaseCloud;
            _notificationCollection = notificationCollection;
            _configuration = configuration;
        }

        public static string DecryptString(string cipherText)
        {
            string key = _configuration["QRKey"];
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public async Task<Response> CheckMergeOrder(int time, Guid customerTripId)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(customerTripId);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(customerTrip.VehicleId);
            var orders = await _unitOfWork.OrderRepository.Query().Where(x => x.CustomerId.Equals(customerTrip.CustomerId) && x.ServiceTypeId.Equals(new Guid(ServiceTypeDefaultData.RENT_SERVICE_ID)))
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            foreach(var x in orders)
            {
                var orderDetails = await _unitOfWork.OrderDetailOfRentingServiceRepository.Query().Where(y => y.LicensePlates.Equals(vehicle.LicensePlates) && y.OrderId.Equals(x.OrderId)).ToListAsync();
                if (orderDetails.Count > 0)
                {
                    int sumQuantity = 0;
                    int sum = 0;
                    PriceOfRentingService priceOfRentingService = null;
                    foreach(var y in orderDetails)
                    {
                        sum += y.ModePrice;
                        if(y.PriceOfRentingServiceId!= null)
                        {
                            sumQuantity += y.Quantity;
                            priceOfRentingService = await _unitOfWork.PriceOfRentingServiceRepository.GetById(y.PriceOfRentingServiceId.Value);
                        }
                    }
                    if (sum > 0)
                    {
                        return new()
                        {
                            StatusCode=400,
                            Message="Không thể gộp hóa đơn"
                        };
                    }
                    else
                    {
                        if (sumQuantity + time > priceOfRentingService.MaxTime)
                        {
                            return new()
                            {
                                StatusCode = 200,
                                Message = "Có thể gộp hóa đơn"
                            };
                        }
                        else
                        {
                            return new()
                            {
                                StatusCode = 400,
                                Message = "Không thể gộp hóa đơn"
                            };
                        }
                    }
                }
            }

            return new()
            {
                StatusCode = 400,
                Message = "Không thể gộp hóa đơn"
            };
        }

        public async Task<Response> CreateCustomerTrip(CustomerTripSearchModel model)
        {
            var customerTrip = new CustomerTrip()
            {
                CustomerTripId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CustomerId = model.CustomerId,
                VehicleId = new Guid(DecryptString(model.VehicleId)),
                RentDeadline = model.RentDeadline,
                Status = (int) CustomerTripStatus.Renting
            };
            var vehicle = await _unitOfWork.VehicleRepository.GetById(new Guid(DecryptString(model.VehicleId)));
            vehicle.Status = (int) VehicleStatus.Renting;
            _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.CustomerTripRepository.Add(customerTrip);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo chuyến đi thành công"
            };
        }

        public async Task<Response> ExtendRentingOrder(ExtendOrderSearchModel model)
        {
            //CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(model.CustomerTripId);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(customerTrip.VehicleId);
            var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(customerTrip.CustomerId)).FirstOrDefaultAsync();
            var orders = await _unitOfWork.OrderRepository.Query().Where(x => x.CustomerId.Equals(customerTrip.CustomerId) && x.ServiceTypeId.Equals(new Guid(ServiceTypeDefaultData.RENT_SERVICE_ID))).OrderByDescending(x => x.CreatedDate).ToListAsync();
            foreach (var x in orders)
            {
                var orderDetails = await _unitOfWork.OrderDetailOfRentingServiceRepository.Query().Where(y => y.LicensePlates.Equals(vehicle.LicensePlates) && y.OrderId.Equals(x.OrderId)).OrderBy(y=> y.PriceOfRentingServiceId).ToListAsync();
                if (orderDetails.Count > 0)
                {
                    if (model.MergeOrder)
                    {
                        for (int i = 1; i < orderDetails.Count; i++)
                        {
                            customerTrip.RentDeadline = customerTrip.RentDeadline.Value.AddHours(-orderDetails[i].Quantity);
                        }
                        customerTrip.RentDeadline = customerTrip.RentDeadline.Value.AddDays(1);
                        _unitOfWork.CustomerTripRepository.Update(customerTrip);
                        decimal transcationAmount = model.Price + orderDetails[0].Price - x.TotalPrice;
                        Transaction transaction = new Transaction()
                        {
                            Amount= transcationAmount,
                            Content="Gộp hóa đơn thuê phương tiện",
                            CreatedDate= DateTime.Now,
                            OrderId= x.OrderId,
                            Status=1,
                            TransactionId= Guid.NewGuid(),
                            WalletId=wallet.WalletId
                        };
                        await _unitOfWork.TransactionRepository.Add(transaction);
                        wallet.AccountBalance += transcationAmount;
                        _unitOfWork.WalletRepository.Update(wallet);
                        x.TotalPrice = model.Price + orderDetails[0].Price;
                        _unitOfWork.OrderRepository.Update(x);
                        OrderDetailOfRentingService orderDetail = new OrderDetailOfRentingService()
                        {
                            Content = model.Content,
                            LicensePlates = vehicle.LicensePlates,
                            ModePrice = model.ModePrice,
                            OrderDetailId = Guid.NewGuid(),
                            OrderId = x.OrderId,
                            Price = model.Price,
                            Quantity = model.Quantity,
                            Status = 1
                        };
                        await _unitOfWork.OrderDetailOfRentingServiceRepository.Add(orderDetail);
                        for(int i=1; i < orderDetails.Count; i++)
                        {
                            await _unitOfWork.OrderDetailOfRentingServiceRepository.Remove(orderDetails[i].OrderDetailId);
                        }
                        break;
                    }
                    else
                    {
                        Transaction transaction = new Transaction()
                        {
                            Amount = model.Price* model.Quantity,
                            Content = "Gia hạn thuê phương tiện",
                            CreatedDate = DateTime.Now,
                            OrderId = x.OrderId,
                            Status = 1,
                            TransactionId = Guid.NewGuid(),
                            WalletId = wallet.WalletId
                        };
                        await _unitOfWork.TransactionRepository.Add(transaction);
                        wallet.AccountBalance -= model.Price * model.Quantity;
                        _unitOfWork.WalletRepository.Update(wallet);
                        x.TotalPrice += model.Price * model.Quantity;
                        _unitOfWork.OrderRepository.Update(x);
                        OrderDetailOfRentingService orderDetail = new OrderDetailOfRentingService()
                        {
                            Content = model.Content,
                            LicensePlates = vehicle.LicensePlates,
                            ModePrice = model.ModePrice,
                            OrderDetailId = Guid.NewGuid(),
                            OrderId= x.OrderId,
                            Price= model.Price,
                            Quantity= model.Quantity,
                            Status=1
                        };
                        await _unitOfWork.OrderDetailOfRentingServiceRepository.Add(orderDetail);
                        if (model.ModePrice == 0)
                        {
                            customerTrip.RentDeadline = customerTrip.RentDeadline.Value.AddHours((double)model.Quantity);
                        }
                        else
                        {
                            customerTrip.RentDeadline = customerTrip.RentDeadline.Value.AddDays((double)model.Quantity);
                        }
                        _unitOfWork.CustomerTripRepository.Update(customerTrip);
                        break;
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            var customer = await _unitOfWork.CustomerRepository.GetById(customerTrip.CustomerId);
            string mes = string.Format("Quý khách đã gia hạn thành công dịch vụ thuê phương tiện {0} đến ngày {1}.", vehicle.LicensePlates, customerTrip.RentDeadline.Value.AddHours(7));
            await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Gia hạn thuê phương tiện", mes);
            SaveNotificationModel noti = new SaveNotificationModel()
            {
                CustomerId = customer.CustomerId.ToString(),
                CustomerFirstName = customer.FirstName,
                CustomerLastName = customer.LastName,
                Title = "Gia hạn thuê phương tiện",
                Message = mes,
                Type = "Renting",
                Status = (int)NotificationStatus.Active
            };
            await _notificationCollection.SaveNotification(noti);
            return new()
            {
                StatusCode = 200,
                Message = "Gia hạn hóa đơn thành công"
            };
        }
        public async Task<PriceRentingViewModel> GetPrice(string id)
        {

            id = DecryptString(id);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(new Guid(id));
            if (vehicle.Status ==1)
            {
                var price = await _unitOfWork.PriceOfRentingServiceRepository.GetById(vehicle.PriceRentingId.Value);
                var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME)).FirstOrDefaultAsync();
                var result = new PriceRentingViewModel()
                {
                    PriceOfRentingServiceId = price.PriceOfRentingServiceId,
                    MaxTime = price.MaxTime,
                    MinTime = price.MinTime,
                    PricePerHour = price.PricePerHour,
                    Status = price.Status,
                    VehicleName = vehicle.Name,
                    Color = vehicle.Color,
                    LicensePlates = vehicle.LicensePlates,
                    PricePerDay = price.FixedPrice,
                    PartnerId = vehicle.PartnerId,
                    ServiceTypeId = serviceType.ServiceTypeId
                };
                var dateNow = DateTime.Now;
                if (dateNow.DayOfWeek == DayOfWeek.Saturday || dateNow.DayOfWeek == DayOfWeek.Sunday)
                {
                    result.PricePerDay = price.WeekendPrice;
                }
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_configuration["Holiday"] + dateNow.Year + "&month=" + dateNow.Month + "&day=" + dateNow.Day),
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    if (body.IndexOf("name") >= 0)
                    {
                        result.PricePerDay = price.HolidayPrice;
                    }
                }
                result.CategoryName = (await _unitOfWork.CategoryRepository.GetById(price.CategoryId)).Name;
                result.PublishYearName = (await _unitOfWork.PublishYearRepository.GetById(price.PublishYearId)).Name;
                return result;
            }
            return null;
        }

        public async Task<PriceRentingViewModel> GetPriceExtend(Guid customerTripId)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(customerTripId);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(customerTrip.VehicleId);
            var price = await _unitOfWork.PriceOfRentingServiceRepository.GetById(vehicle.PriceRentingId.Value);
            var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME)).FirstOrDefaultAsync();
            var result = new PriceRentingViewModel()
            {
                PriceOfRentingServiceId = price.PriceOfRentingServiceId,
                MaxTime = price.MaxTime,
                MinTime = price.MinTime,
                PricePerHour = price.PricePerHour,
                Status = price.Status,
                VehicleName = vehicle.Name,
                Color = vehicle.Color,
                LicensePlates = vehicle.LicensePlates,
                PricePerDay = price.FixedPrice,
                PartnerId = vehicle.PartnerId,
                ServiceTypeId = serviceType.ServiceTypeId
            };
            var dateNow = DateTime.Now;
            if (dateNow.DayOfWeek == DayOfWeek.Saturday || dateNow.DayOfWeek == DayOfWeek.Sunday)
            {
                result.PricePerDay = price.WeekendPrice;
            }
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_configuration["holiday"] + dateNow.Year + "&month=" + dateNow.Month + "&day=" + dateNow.Day),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                if (body.IndexOf("name") >= 0)
                {
                    result.PricePerDay = price.HolidayPrice;
                }
            }
            result.CategoryName = (await _unitOfWork.CategoryRepository.GetById(price.CategoryId)).Name;
            result.PublishYearName = (await _unitOfWork.PublishYearRepository.GetById(price.PublishYearId)).Name;
            return result;
        }

        public async Task<Response> ReturnVehicle(ReturnVehicleSearchModel model)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(model.CustomerTripId);
            if (customerTrip == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Yêu cầu trả phương tiện thất bại"
                };
            }
            if (customerTrip.Status == (int)CustomerTripStatus.Renting)
            {
                customerTrip.Status = (int)CustomerTripStatus.Requesting;
                customerTrip.ReturnVehicleStationId = new Guid(DecryptString(model.RentStationId));
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
                await _unitOfWork.SaveChangesAsync();
            }
            return new()
            {
                StatusCode = 200,
                Message = "Yêu cầu trả phương tiện thành công"
            };
        }
    }
}