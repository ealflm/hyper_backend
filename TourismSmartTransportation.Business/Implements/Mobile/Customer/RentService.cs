using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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
    public class RentService : BaseService, IRentService
    {
        public RentService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public static string DecryptString(string cipherText)
        {
            string key = "b14pa58l8aee4133bhce2ea2315b1916";
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

        public async Task<Response> CreateCustomerTrip(CustomerTripSearchModel model)
        {
            var customerTrip = new CustomerTrip()
            {
                CustomerTripId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CustomerId = model.CustomerId,
                VehicleId = model.VehicleId,
                RentDeadline = model.RentDeadline,
                Status = (int) CustomerTripStatus.Renting
            };
            var vehicle = await _unitOfWork.VehicleRepository.GetById(model.VehicleId);
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

        public async Task<PriceRentingViewModel> GetPrice(string id)
        {

            id = DecryptString(id);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(new Guid(id));
            if (vehicle.Status != 3)
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
                    RequestUri = new Uri("https://holidays.abstractapi.com/v1/?api_key=ba043ce3a5274588ae52c2b1c4a6aebc&country=VN&year=" + dateNow.Year + "&month=" + dateNow.Month + "&day=" + dateNow.Day),
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

        public async Task<Response> ReturnVehicle(ReturnVehicleSearchModel model)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(model.CustomerTripId);
            if (customerTrip == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Trả phương tiện thất bại"
                };
            }
            if(customerTrip.Status == (int)CustomerTripStatus.Overdue)
            {
                customerTrip.Status = (int)CustomerTripStatus.Done;
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(customerTrip.VehicleId);
                var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Equals("Thuê xe")).FirstOrDefaultAsync();
                var orderRentList = await _unitOfWork.OrderRepository.Query().Where(x => x.CustomerId.Equals(customerTrip.CustomerId) && x.PartnerId.Equals(vehicle.PartnerId) && x.ServiceTypeId.Equals(serviceType.ServiceTypeId)).OrderByDescending(x => x.CreatedDate).ToListAsync();
                List<OrderDetailOfRentingService> orderDetailList = new List<OrderDetailOfRentingService>();
                Order oldOrder = null;
                foreach (Order order in orderRentList)
                {
                    orderDetailList = await _unitOfWork.OrderDetailOfRentingServiceRepository.Query().Where(x => x.OrderId.Equals(order.OrderId) && x.LicensePlates.Equals(vehicle.LicensePlates)).ToListAsync();
                    if (orderDetailList.Count != 0)
                    {
                        oldOrder = order;
                        break;
                    }
                }
                decimal returnPrice = orderDetailList[0].PriceOfRentingServiceId != null ? orderDetailList[1].Price : orderDetailList[0].Price;
                decimal bonusPrice = oldOrder.TotalPrice- returnPrice;
                bonusPrice = bonusPrice * 0.1M;
                var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(customerTrip.CustomerId)).FirstOrDefaultAsync();

                // Tạo transaction khi customer sử dụng dịch vụ
                var transaction = new Transaction()
                {
                    TransactionId = Guid.NewGuid(),
                    Content = "Hoàn phí thu hồi phương tiện",
                    OrderId = oldOrder.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount = returnPrice,
                    Status = 1,
                    WalletId = wallet.WalletId
                };

                // Cập nhật lại ví của customer
                wallet.AccountBalance += returnPrice;
                await _unitOfWork.TransactionRepository.Add(transaction);
                    // Lấy ví của partner
                var partnerWallet = await _unitOfWork.WalletRepository
                                          .Query()
                                          .Where(x => x.PartnerId == oldOrder.PartnerId)
                                          .FirstOrDefaultAsync();

                    // Tạo transaction cho partner nhận tiền từ giao dịch sử dụng dịch vụ của customer
                    var partnerTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Đối tác trả lại 90% phí thu hồi phương tiện",
                        OrderId = oldOrder.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -(returnPrice * 0.9M),
                        Status = 1,
                        WalletId = partnerWallet.WalletId
                    };
                    // Cập nhật lại ví của partner
                    partnerWallet.AccountBalance -= returnPrice* 0.9M;
                    await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                    _unitOfWork.WalletRepository.Update(partnerWallet);

                // Lấy ví của admin
                var adminWallet = await _unitOfWork.WalletRepository
                                    .Query()
                                    .Where(x => x.PartnerId == null && x.CustomerId == null)
                                    .FirstOrDefaultAsync();

                // Tạo transaction cho admin nhận tiền từ giao dịch sử dụng dịch vụ của customer
                var adminTransaction = new Transaction()
                {
                    TransactionId = Guid.NewGuid(),
                    Content = $"Hệ thống trả lại 10% phí thu hồi phương tiện",
                    OrderId = oldOrder.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount =-( returnPrice * 0.1M),
                    Status = 1,
                    WalletId = adminWallet.WalletId
                };

                adminWallet.AccountBalance -= returnPrice * 0.1M;
                await _unitOfWork.TransactionRepository.Add(adminTransaction);
                _unitOfWork.WalletRepository.Update(adminWallet);

                Guid rentStationId = new Guid(DecryptString(model.RentStationId));
                if (vehicle.RentStationId.Equals(rentStationId))
                {
                    var bonusTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = "Thưởng thêm phí hoàn trả phương tiện đúng trạm 10% hóa đơn thuê phương tiện không tín phí thu hồi",
                        OrderId = oldOrder.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = bonusPrice,
                        Status = 1,
                        WalletId = wallet.WalletId
                    };
                    wallet.AccountBalance += bonusPrice;
                    await _unitOfWork.TransactionRepository.Add(bonusTransaction);
                }

                _unitOfWork.WalletRepository.Update(wallet);
                await _unitOfWork.SaveChangesAsync();


            }

            return new()
            {
                StatusCode = 200,
                Message = "Trả phương tiện thành công"
            };
        }
    }
}