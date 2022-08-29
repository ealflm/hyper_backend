using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class CustomerTripService : BaseService, ICustomerTripService
    {
        private IFirebaseCloudMsgService _firebaseCloud;
        private INotificationCollectionService _notificationCollection;
        private static IConfiguration _configuration;
        public CustomerTripService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IFirebaseCloudMsgService firebaseCloud, INotificationCollectionService notificationCollection, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            _firebaseCloud = firebaseCloud;
            _notificationCollection = notificationCollection;
            _configuration = configuration;
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTrips()
        {
            var customerTrips = await _unitOfWork.CustomerTripRepository.Query().OrderByDescending(x => x.CreatedDate).Select(x => x.AsCustomerTripViewModel()).ToListAsync();
            foreach (CustomerTripViewModel x in customerTrips)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                var customer = await _unitOfWork.CustomerRepository.GetById(x.CustomerId);
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId);
                x.VehicleName = vehicle.Name;
                x.LicensePlates = vehicle.LicensePlates;
                x.ServiceTypeName = serviceType.Name;
                x.CustomerName = customer.FirstName + " " + customer.LastName;
            }
            return customerTrips;
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTrips(Guid partnerId)
        {
            var customerTrips = await _unitOfWork.CustomerTripRepository.Query().OrderByDescending(x => x.CreatedDate).Select(x => x.AsCustomerTripViewModel()).ToListAsync();
            foreach (CustomerTripViewModel x in customerTrips)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                var customer = await _unitOfWork.CustomerRepository.GetById(x.CustomerId);
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId);
                x.VehicleName = vehicle.Name;
                x.LicensePlates = vehicle.LicensePlates;
                x.ServiceTypeName = serviceType.Name;
                x.CustomerName = customer.FirstName + " " + customer.LastName;
                if (!vehicle.PartnerId.Equals(partnerId))
                {
                    customerTrips.Remove(x);
                }
            }
            return customerTrips;
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTripsForDriver(Guid driverId)
        {
            var driver = await _unitOfWork.DriverRepository.GetById(driverId);
            var customerTrips = await _unitOfWork.CustomerTripRepository.Query().Where(x => x.VehicleId.Equals(driver.VehicleId)).OrderByDescending(x => x.CreatedDate).Select(x => x.AsCustomerTripViewModel()).ToListAsync();

            foreach (CustomerTripViewModel x in customerTrips)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                var customer = await _unitOfWork.CustomerRepository.GetById(x.CustomerId);
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId);
                x.VehicleName = vehicle.Name;
                x.LicensePlates = vehicle.LicensePlates;
                x.ServiceTypeName = serviceType.Name;
                x.CustomerName = customer.FirstName + " " + customer.LastName;
            }
            return customerTrips;
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTripsListForRentingService(CustomerTripSearchModel model)
        {
            var customerTripsList = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.Status == (int)CustomerTripStatus.Renting)
                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                        customerTrip => customerTrip.VehicleId,
                                        vehicle => vehicle.VehicleId,
                                        (customerTrip, vehicle) => new
                                        {
                                            customerTrip,
                                            vehicle
                                        }
                                    )
                                    .Join(_unitOfWork.ServiceTypeRepository.Query(),
                                        _ => _.vehicle.ServiceTypeId,
                                        serviceType => serviceType.ServiceTypeId,
                                        (_, serviceType) => new
                                        {
                                            CustomerTrip = _.customerTrip,
                                            Vehicle = _.vehicle,
                                            ServiceType = serviceType
                                        }
                                    )
                                    .Where(x => x.ServiceType.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME))
                                    .Select(x => x.CustomerTrip.AsCustomerTripViewModel())
                                    .ToListAsync();

            return customerTripsList;
        }

        public async Task<Response> UpdateStatusCustomerTrip(Guid id, CustomerTripSearchModel model)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(id);
            if (customerTrip == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            customerTrip.Status = model.Status;
            _unitOfWork.CustomerTripRepository.Update(customerTrip);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công"
            };
        }


        public async Task<Response> ReturnVehicle(Guid customerTripId)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(customerTripId);
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

            if (customerTrip.Status == (int)CustomerTripStatus.Overdue)
            {
                customerTrip.Status = (int)CustomerTripStatus.Done;
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
                //var vehicle = await _unitOfWork.VehicleRepository.GetById(customerTrip.VehicleId);
                vehicle.Status = (int)VehicleStatus.Ready;
                _unitOfWork.VehicleRepository.Update(vehicle);
                oldOrder.Status = (int)OrderStatus.Done;
                _unitOfWork.OrderRepository.Update(oldOrder);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {


                decimal returnPrice = orderDetailList[0].PriceOfRentingServiceId != null ? orderDetailList[1].Price : orderDetailList[0].Price;
                decimal bonusPrice = oldOrder.TotalPrice - returnPrice;
                bonusPrice = bonusPrice * decimal.Parse(_configuration["Admin"]);
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
                    Content = $"Đối tác trả lại " + (decimal.Parse(_configuration["Partner"]) * 100) + "% phí thu hồi phương tiện",
                    OrderId = oldOrder.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount = -(returnPrice * decimal.Parse(_configuration["Partner"])),
                    Status = 1,
                    WalletId = partnerWallet.WalletId
                };
                // Cập nhật lại ví của partner
                partnerWallet.AccountBalance -= returnPrice * decimal.Parse(_configuration["Partner"]);
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
                    Content = $"Hệ thống trả lại " + (decimal.Parse(_configuration["Admin"]) * 100) + "% phí thu hồi phương tiện",
                    OrderId = oldOrder.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount = -(returnPrice * decimal.Parse(_configuration["Admin"])),
                    Status = 1,
                    WalletId = adminWallet.WalletId
                };

                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                adminWallet.AccountBalance -= returnPrice * decimal.Parse(_configuration["Admin"]);
                await _unitOfWork.TransactionRepository.Add(adminTransaction);
                _unitOfWork.WalletRepository.Update(adminWallet);
                var customer = await _unitOfWork.CustomerRepository.GetById(customerTrip.CustomerId);
                Guid rentStationId = customerTrip.ReturnVehicleStationId.Value;
                if (vehicle.RentStationId.Equals(rentStationId))
                {
                    var bonusTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = "Thưởng thêm phí hoàn trả phương tiện đúng trạm " + (decimal.Parse(_configuration["Admin"]) * 100) + "% hóa đơn thuê phương tiện không tín phí thu hồi",
                        OrderId = oldOrder.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = bonusPrice,
                        Status = 1,
                        WalletId = wallet.WalletId
                    };
                    wallet.AccountBalance += bonusPrice;
                    await _unitOfWork.TransactionRepository.Add(bonusTransaction);
                    var mesBonusPrice = string.Format(elGR, "Quý khách được thưởng thêm phí hoàn trả phương tiện đúng trạm {0:N0} VNĐ 10% hóa đơn thuê phương tiện không tín phí thu hồi", bonusPrice);
                    await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Phần thưởng", mesBonusPrice);
                    SaveNotificationModel notiBonus = new SaveNotificationModel()
                    {
                        CustomerId = customer.CustomerId.ToString(),
                        CustomerFirstName = customer.FirstName,
                        CustomerLastName = customer.LastName,
                        Title = "Phần thưởng",
                        Message = mesBonusPrice,
                        Type = "Bonus",
                        Status = (int)NotificationStatus.Active
                    };
                    await _notificationCollection.SaveNotification(notiBonus);
                }

                _unitOfWork.WalletRepository.Update(wallet);
                vehicle.Status = (int)VehicleStatus.Ready;
                customerTrip.Status = (int)CustomerTripStatus.Done;
                _unitOfWork.VehicleRepository.Update(vehicle);
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
                oldOrder.Status = (int)OrderStatus.Done;
                _unitOfWork.OrderRepository.Update(oldOrder);
                await _unitOfWork.SaveChangesAsync();
                var mesReturnPrice = string.Format(elGR, "Quý khách vừa được hoàn {0:N0} VNĐ phí thu hồi phương tiện", returnPrice);
                await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Hoàn phí thu hồi phương tiện", mesReturnPrice);
                SaveNotificationModel noti = new SaveNotificationModel()
                {
                    CustomerId = customer.CustomerId.ToString(),
                    CustomerFirstName = customer.FirstName,
                    CustomerLastName = customer.LastName,
                    Title = "Hoàn phí thu hồi phương tiện",
                    Message = mesReturnPrice,
                    Type = "Refund",
                    Status = (int)NotificationStatus.Active
                };
                await _notificationCollection.SaveNotification(noti);


            }

            return new()
            {
                StatusCode = 200,
                Message = "Trả phương tiện thành công"
            };
        }
    }
}