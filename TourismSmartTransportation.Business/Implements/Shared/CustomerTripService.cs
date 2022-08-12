using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class CustomerTripService : BaseService, ICustomerTripService
    {
        public CustomerTripService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTrips()
        {
            var customerTrips = await _unitOfWork.CustomerTripRepository.Query().OrderByDescending(x => x.CreatedDate).Select(x => x.AsCustomerTripViewModel()).ToListAsync();
            foreach(CustomerTripViewModel x in customerTrips)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId);
                x.VehicleName = vehicle.Name;
                x.LicensePlates = vehicle.LicensePlates;
                x.ServiceTypeName = serviceType.Name;
            }
            return customerTrips;
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTrips(Guid partnerId)
        {
            var customerTrips = await _unitOfWork.CustomerTripRepository.Query().OrderByDescending(x => x.CreatedDate).Select(x => x.AsCustomerTripViewModel()).ToListAsync();
            foreach (CustomerTripViewModel x in customerTrips)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId);
                x.VehicleName = vehicle.Name;
                x.LicensePlates = vehicle.LicensePlates;
                x.ServiceTypeName = serviceType.Name;
                if (!vehicle.PartnerId.Equals(partnerId))
                {
                    customerTrips.Remove(x);
                }
            }
            return customerTrips; 
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTripsListForRentingService(CustomerTripSearchModel model)
        {
            var customerTripsList = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.Status == 1)
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
            if (customerTrip.Status == (int)CustomerTripStatus.Overdue)
            {
                customerTrip.Status = (int)CustomerTripStatus.Done;
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
                var vehicle = await _unitOfWork.VehicleRepository.GetById(customerTrip.VehicleId);
                vehicle.Status = (int)VehicleStatus.Ready;
                _unitOfWork.VehicleRepository.Update(vehicle);
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
                decimal bonusPrice = oldOrder.TotalPrice - returnPrice;
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
                partnerWallet.AccountBalance -= returnPrice * 0.9M;
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
                    Amount = -(returnPrice * 0.1M),
                    Status = 1,
                    WalletId = adminWallet.WalletId
                };

                adminWallet.AccountBalance -= returnPrice * 0.1M;
                await _unitOfWork.TransactionRepository.Add(adminTransaction);
                _unitOfWork.WalletRepository.Update(adminWallet);

                Guid rentStationId = customerTrip.ReturnVehicleStationId.Value;
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
                vehicle.Status = (int)VehicleStatus.Ready;
                _unitOfWork.VehicleRepository.Update(vehicle);
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