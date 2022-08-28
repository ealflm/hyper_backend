using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class OrderHelpersService : BaseService, IOrderHelpersService
    {
        private readonly IPackageService _packageService;
        private IFirebaseCloudMsgService _firebaseCloud;
        private INotificationCollectionService _notificationCollection;
        private IConfiguration _configuration;
        public OrderHelpersService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IPackageService packageService, IFirebaseCloudMsgService firebaseCloud, INotificationCollectionService notificationCollection, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            _packageService = packageService;
            _firebaseCloud = firebaseCloud;
            _notificationCollection = notificationCollection;
            _configuration = configuration;
        }

        /// <summary>
        /// Create a order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> CreateOrder(CreateOrderModel order)
        {
            var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(order.CustomerId)).FirstOrDefaultAsync();
            var customer = await _unitOfWork.CustomerRepository.GetById(order.CustomerId);
            if (wallet.AccountBalance < order.TotalPrice)
            {
                var mesWallet = "Số dư trong tài khoản của quý khách không không đủ để thực hiện dịch vụ";
                await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Thông báo số dư", mesWallet);
                SaveNotificationModel notiWlalet = new SaveNotificationModel()
                {
                    CustomerId = customer.CustomerId.ToString(),
                    CustomerFirstName = customer.FirstName,
                    CustomerLastName = customer.LastName,
                    Title = "Thông báo số dư",
                    Message = mesWallet,
                    Type = "Wallet",
                    Status = (int)NotificationStatus.Active
                };
                await _notificationCollection.SaveNotification(notiWlalet);
                return new()
                {
                    StatusCode = 400,
                    Message = "Số tiền trong ví không đủ!"
                };
            }
            var newOrder = new Order()
            {
                OrderId = Guid.NewGuid(),
                CustomerId = order.CustomerId,
                ServiceTypeId = order.ServiceTypeId != null ? order.ServiceTypeId.Value : null,
                DiscountId = order.DiscountId != null ? order.DiscountId.Value : null,
                CreatedDate = DateTime.UtcNow,
                TotalPrice = order.TotalPrice,
                PartnerId = order.PartnerId,
                Status = (int)OrderStatus.WrongStatus,
            };

            string content = "";
            bool isUsingService = true;
            OrderDetailOfBookingService tempOrderDetailOfBooking = null;

            foreach (OrderDetailsInfo x in order.OrderDetailsInfos)
            {
                // Customer buy package
                if (x.PackageId != null)
                {

                    var orderDetail = new OrderDetailOfPackage()
                    {
                        OrderId = newOrder.OrderId,
                        PackageId = x.PackageId.Value,
                        Price = x.Price,
                        Content = x.Content,
                        Quantity = x.Quantity,
                        Status = 1
                    };
                    await _unitOfWork.OrderDetailOfPackageRepository.Add(orderDetail);
                    content = x.Content;
                    newOrder.Status = (int)OrderStatus.Done; // Cập nhật lại trạng thái order đổi với service 'mua gói dịch vụ' 
                    isUsingService = false;
                }
                // Customer using service
                else
                {
                    var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(order.ServiceTypeId.Value);
                    if (serviceType == null)
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = "Yêu cầu không hợp lệ!"
                        };
                    }

                    if (serviceType.Name.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME))
                    {
                        var orderDetail = new OrderDetailOfBusService()
                        {
                            OrderId = newOrder.OrderId,
                            PriceOfBusServiceId = x.PriceOfBusServiceId.Value,
                            Price = x.Price,
                            Content = x.Content,
                            Quantity = x.Quantity,
                            Status = 1
                        };
                        await _unitOfWork.OrderDetailOfBusServiceRepository.Add(orderDetail);
                        content = $"Sử dụng {ServiceTypeDefaultData.BUS_SERVICE_CONTENT}";
                        newOrder.Status = (int)OrderStatus.Paid; // Cập nhật lại trạng thái order đổi với service 'Đi xe buýt' 
                    }
                    else if (serviceType.Name.Contains(ServiceTypeDefaultData.BOOK_SERVICE_NAME))
                    {
                        tempOrderDetailOfBooking = new OrderDetailOfBookingService()
                        {
                            OrderId = newOrder.OrderId,
                            PriceOfBookingServiceId = x.PriceOfBookingServiceId.Value,
                            Price = x.Price,
                            Content = x.Content,
                            Quantity = x.Quantity,
                            Status = 1
                        };
                        // await _unitOfWork.OrderDetailOfBookingServiceRepository.Add(orderDetail);
                        content = $"Sử dụng {ServiceTypeDefaultData.BOOK_SERVICE_CONTENT}";
                        newOrder.Status = (int)OrderStatus.Paid; // Cập nhật lại trạng thái order đổi với service 'đặt xe' 
                        // await _unitOfWork.OrderRepository.Add(newOrder);
                        // await _unitOfWork.SaveChangesAsync();
                        // return new()
                        // {
                        //     StatusCode = 201,
                        //     Message = "Đã tạo hóa đơn với trạng thái chưa thanh toán cho dịch vụ đặt xe"
                        // };
                    }
                    else if (serviceType.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME))
                    {
                        var orderDetail = new OrderDetailOfRentingService()
                        {
                            OrderId = newOrder.OrderId,
                            PriceOfRentingServiceId = x.PriceOfRentingServiceId.Value,
                            Price = x.Price,
                            Content = x.Content,
                            Quantity = x.Quantity,
                            LicensePlates = x.LicensePlates,
                            ModePrice = x.ModePrice,
                            Status = 1
                        };
                        await _unitOfWork.OrderDetailOfRentingServiceRepository.Add(orderDetail);
                        content = $"Sử dụng {ServiceTypeDefaultData.RENT_SERVICE_CONTENT}";
                        newOrder.Status = (int)OrderStatus.Paid; // Cập nhật lại trạng thái order đổi với service 'thuê xe' 
                    }
                    else
                    {
                        // more other service type 
                    }


                }
            }


            // Xử lý mã giảm giá cho hóa đơn
            if (newOrder.DiscountId != null)
            {
                var discount = await _unitOfWork.DiscountRepository.GetById(newOrder.DiscountId.Value);
                if (discount.ServiceTypeId != null) // kiểm tra mã giảm giá này sử dụng cho dịch vào cụ thể nào hay cho tất cả dịch vụ
                {
                    if (discount.ServiceTypeId.Value != newOrder.ServiceTypeId)
                    {
                        var serviceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(discount.ServiceTypeId.Value)).Name;
                        return new()
                        {
                            StatusCode = 400,
                            Message = $"Mã giảm giá này chỉ sử dụng cho dịch vụ {serviceTypeName}"
                        };
                    }
                }
                newOrder.TotalPrice -= newOrder.TotalPrice * discount.Value; // trừ tiền giảm giá
            }


            dynamic currentPackageIsUsed = null;
            bool isUsingPackage = false;
            decimal orderPrice = 0;
            decimal newOrderTotalPrice = newOrder.TotalPrice;
            if (isUsingService) // kiểm tra có sử dụng package hay không
            {
                currentPackageIsUsed = await _packageService.GetCurrentPackageIsUsed(newOrder.CustomerId);
                if (currentPackageIsUsed != null && order.Distance != null && order.Distance < (currentPackageIsUsed.LimitDistances - currentPackageIsUsed.CurrentDistances))
                {
                    dynamic distancesNow = null;
                    dynamic cardSwipesNow = null;
                    dynamic numberOfTripsNow = null;

                    if (newOrder.ServiceTypeId != null &&
                        newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BUS_SERVICE_ID))
                    {
                        distancesNow = currentPackageIsUsed.CurrentDistances + order.Distance;
                        cardSwipesNow = currentPackageIsUsed.CurrentCardSwipes + 1;
                    }
                    else if (newOrder.ServiceTypeId != null &&
                                newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BOOK_SERVICE_ID))
                    {
                        numberOfTripsNow = currentPackageIsUsed.CurrentNumberOfTrips + 1;
                    }


                    // Sử dụng package cho trường hợp đi xe buýt
                    if (newOrder.ServiceTypeId != null &&
                        newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BUS_SERVICE_ID) &&
                        distancesNow <= currentPackageIsUsed.LimitDistances &&
                        cardSwipesNow <= currentPackageIsUsed.LimitCardSwipes)
                    {
                        orderPrice = newOrder.TotalPrice;
                        newOrder.TotalPrice = 0;
                        isUsingPackage = true;
                    }
                    // Sử dụng package cho trường hợp đặt xe
                    else if (newOrder.ServiceTypeId != null &&
                                newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BOOK_SERVICE_ID) &&
                                numberOfTripsNow <= currentPackageIsUsed.LimitNumberOfTrips)
                    {
                        orderPrice = newOrder.TotalPrice - newOrder.TotalPrice * (currentPackageIsUsed.DiscountValueTrip * 0.01M);
                        newOrder.TotalPrice = orderPrice;
                        isUsingPackage = true;
                        tempOrderDetailOfBooking.Price = orderPrice;
                        newOrderTotalPrice = orderPrice;
                    }
                }
            }

            // cập nhật order detail cho dịch vụ booking xuông db.
            if (tempOrderDetailOfBooking != null)
            {
                await _unitOfWork.OrderDetailOfBookingServiceRepository.Add(tempOrderDetailOfBooking);
            }


            // Tạo transaction khi customer sử dụng dịch vụ
            var transaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                Content = content,
                OrderId = newOrder.OrderId,
                CreatedDate = newOrder.CreatedDate,
                Amount = -newOrderTotalPrice,
                Status = 1,
                WalletId = wallet.WalletId
            };

            // Cập nhật lại ví của customer
            wallet.AccountBalance -= newOrder.TotalPrice;
            await _unitOfWork.TransactionRepository.Add(transaction);
            _unitOfWork.WalletRepository.Update(wallet);
            await _unitOfWork.OrderRepository.Add(newOrder);


            if (isUsingPackage)
            {
                if (newOrder.ServiceTypeId != null && newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BUS_SERVICE_ID))
                {
                    var transactionRefund = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = "Hoàn tiền từ gói dịch vụ",
                        OrderId = newOrder.OrderId,
                        CreatedDate = newOrder.CreatedDate,
                        Amount = newOrderTotalPrice,
                        Status = 1,
                        WalletId = wallet.WalletId
                    };
                    await _unitOfWork.TransactionRepository.Add(transactionRefund);
                }
            }



            // Trường hợp sử dụng dịch vụ thì cập nhật ví cho partner
            if (isUsingService)
            {
                // Lấy ví của partner
                var partnerWallet = await _unitOfWork.WalletRepository
                                                .Query()
                                                .Where(x => x.PartnerId == newOrder.PartnerId)
                                                .FirstOrDefaultAsync();

                // Tạo transaction cho partner nhận tiền từ giao dịch sử dụng dịch vụ của customer
                var partnerTransaction = new Transaction()
                {
                    TransactionId = Guid.NewGuid(),
                    Content = $"Đối tác nhận " + (decimal.Parse(_configuration["Partner"]) * 100) + "% hóa đơn",
                    OrderId = newOrder.OrderId,
                    CreatedDate = DateTime.UtcNow,
                    Amount = newOrder.TotalPrice * decimal.Parse(_configuration["Partner"]),
                    Status = 1,
                    WalletId = partnerWallet.WalletId
                };

                if (isUsingPackage && tempOrderDetailOfBooking == null)
                {
                    // nhận toàn bộ tiền trip từ khách hàng trước và sẽ đưa phần trăm lại cho admin sau khi mà customer hoàn thành trip
                    partnerTransaction.Amount = orderPrice;
                }

                // Cập nhật lại ví của partner
                partnerWallet.AccountBalance += partnerTransaction.Amount;
                await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                _unitOfWork.WalletRepository.Update(partnerWallet);
            }

            // Lấy ví của admin
            var adminWallet = await _unitOfWork.WalletRepository
                                .Query()
                                .Where(x => x.PartnerId == null && x.CustomerId == null)
                                .FirstOrDefaultAsync();

            // Tạo transaction cho admin nhận tiền từ giao dịch sử dụng dịch vụ của customer
            var adminTransaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                Content = $"Hệ thống nhận " + (decimal.Parse(_configuration["Admin"]) * 100) + "% hóa đơn",
                OrderId = newOrder.OrderId,
                CreatedDate = DateTime.UtcNow,
                Amount = newOrder.TotalPrice * decimal.Parse(_configuration["Admin"]),
                Status = 1,
                WalletId = adminWallet.WalletId
            };

            // Trường hợp mua gói dịch vụ thì thêm toàn bộ tiền vào ví admin
            if (!isUsingService)
            {
                adminTransaction.Content = "Hệ thống nhận tiền từ hóa đơn mua gói dịch vụ";
                adminTransaction.Amount = newOrder.TotalPrice;
            }

            // Cập nhật lại ví của admin
            adminWallet.AccountBalance += adminTransaction.Amount;

            // Cập nhật lại ví trong trường hợp khách hàng có sử dụng dịch vụ xe buýt
            if (isUsingPackage && tempOrderDetailOfBooking == null)
            {
                // chuyển toàn bộ tiền của trip cho đối tác trước và nhận refund price từ đối tác lại sau khi customer hoàn thành trip
                adminTransaction.Amount = -orderPrice;
                adminWallet.AccountBalance += adminTransaction.Amount;
                adminTransaction.Content = "Hệ thống chuyển tiền cho đối tác";
            }
            await _unitOfWork.TransactionRepository.Add(adminTransaction);
            _unitOfWork.WalletRepository.Update(adminWallet);

            await _unitOfWork.SaveChangesAsync();
            var serviceTypeNoti = await _unitOfWork.ServiceTypeRepository.GetById(order.ServiceTypeId.Value);

            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            string mes = string.Format(elGR, content + " với hóa đơn {0:N0} VNĐ", order.TotalPrice);
            if (isUsingPackage)
            {
                if (newOrder.ServiceTypeId != null && newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BUS_SERVICE_ID))
                {
                    mes = "Quý khách vừa sử dụng 1 lần đi xe buýt";
                }
                else
                {
                    currentPackageIsUsed = await _packageService.GetCurrentPackageIsUsed(newOrder.CustomerId);
                    mes = string.Format(elGR, content + " có áp dụng gói dịch vụ giảm {0:N0}% và hóa đơn thanh toán là {1:N0} VNĐ",
                                                currentPackageIsUsed.DiscountValueTrip,
                                                tempOrderDetailOfBooking.Price);
                    // mes += string.Format(" .", (currentPackageIsUsed.DiscountValueTrip));
                }

            }
            await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, serviceTypeNoti.Content, mes);
            SaveNotificationModel noti = new SaveNotificationModel()
            {
                CustomerId = customer.CustomerId.ToString(),
                CustomerFirstName = customer.FirstName,
                CustomerLastName = customer.LastName,
                Title = serviceTypeNoti.Content,
                Message = mes,
                Type = serviceTypeNoti.Name,
                Status = (int)NotificationStatus.Active
            };
            await _notificationCollection.SaveNotification(noti);

            return new()
            {
                StatusCode = 201,
                Message = "Tạo hóa đơn thành công!"
            };
        }

        public async Task<Response> ProcessBooking(ProcessBookingModel model)
        {
            // Xử lý cho hóa đơn
            if (model.ChangeOrder)
            {
                var order = await _unitOfWork.OrderRepository
                            .Query()
                            .Where(x => x.ServiceTypeId.ToString() == ServiceTypeDefaultData.BOOK_SERVICE_ID)
                            .Where(x => x.CustomerId.ToString() == model.CustomerId)
                            .Where(x => x.Status != (int)OrderStatus.Canceled && x.Status != (int)OrderStatus.Done)
                            .FirstOrDefaultAsync();

                order.Status = model.OrderStatus;

                // Chỉ tiến hành thanh toán khi tài xế đến đón khách
                if (model.OrderStatus == (int)OrderStatus.Paid && model.CustomerTripStatus == (int)CustomerTripStatus.PickedUp)
                {
                    // Sử dụng gói dịch vụ
                    var currentPackageIsUsed = await _packageService.GetCurrentPackageIsUsed(order.CustomerId);
                    if (currentPackageIsUsed != null)
                    {
                        var numberOfTripsNow = currentPackageIsUsed.CurrentNumberOfTrips + 1;
                        if (numberOfTripsNow <= currentPackageIsUsed.LimitNumberOfTrips)
                        {
                            order.TotalPrice -= (order.TotalPrice * currentPackageIsUsed.DiscountValueTrip);
                        }
                    }

                    // kiểm tra ví trước khi thánh toán
                    var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(order.CustomerId)).FirstOrDefaultAsync();
                    if (wallet.AccountBalance < order.TotalPrice)
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = "Số tiền trong ví không đủ!"
                        };
                    }

                    // Tạo giao dịch khi tài xế đã đến đón khách hàng
                    var transaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = "Sử dụng dịch vụ đặt xe",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.UtcNow,
                        Amount = -order.TotalPrice,
                        Status = 1,
                        WalletId = wallet.WalletId
                    };

                    // cập nhật lại ví của khách hàng
                    wallet.AccountBalance -= order.TotalPrice;
                    await _unitOfWork.TransactionRepository.Add(transaction);
                    _unitOfWork.WalletRepository.Update(wallet);



                    // Lấy ví của partner
                    var partnerWallet = await _unitOfWork.WalletRepository
                                                    .Query()
                                                    .Where(x => x.PartnerId == order.PartnerId)
                                                    .FirstOrDefaultAsync();

                    // Tạo transaction cho partner nhận tiền từ giao dịch sử dụng dịch vụ của customer
                    var partnerTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Đối tác nhận " + (decimal.Parse(_configuration["Partner"]) * 100) + "% hóa đơn",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.UtcNow,
                        Amount = order.TotalPrice * decimal.Parse(_configuration["Partner"]),
                        Status = 1,
                        WalletId = partnerWallet.WalletId
                    };

                    // Cập nhật lại ví của partner
                    partnerWallet.AccountBalance += partnerTransaction.Amount;
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
                        Content = $"Hệ thống nhận " + (decimal.Parse(_configuration["Admin"]) * 100) + "% hóa đơn",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.UtcNow,
                        Amount = order.TotalPrice * decimal.Parse(_configuration["Admin"]),
                        Status = 1,
                        WalletId = adminWallet.WalletId
                    };

                    // Cập nhật lại ví của admin
                    adminWallet.AccountBalance += adminTransaction.Amount;
                    await _unitOfWork.TransactionRepository.Add(adminTransaction);
                    _unitOfWork.WalletRepository.Update(adminWallet);


                }
                // cập nhật hóa đơn
                _unitOfWork.OrderRepository.Update(order);
            }


            // Xử lý cho customer trip
            if (model.ChangeCustomerTrip)
            {
                var customerTrip = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.CustomerId.ToString() == model.CustomerId)
                                    .Where(x => x.Status == (int)CustomerTripStatus.New ||
                                                x.Status == (int)CustomerTripStatus.Accepted ||
                                                x.Status == (int)CustomerTripStatus.PickedUp)
                                    .FirstOrDefaultAsync();

                customerTrip.Status = model.CustomerTripStatus;
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
            }

            if (model.ChangeCustomerTrip == false && model.ChangeOrder == false)
            {
                return new()
                {
                    StatusCode = 304,
                    Message = "Không có sự thay đổi"
                };
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công"
            };
        }
    }
}