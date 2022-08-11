using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class OrderHelpersService : BaseService, IOrderHelpersService
    {
        private readonly IPackageService _packageService;

        public OrderHelpersService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IPackageService packageService) : base(unitOfWork, blobServiceClient)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Create a order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> CreateOrder(CreateOrderModel order)
        {
            var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(order.CustomerId)).FirstOrDefaultAsync();
            if (wallet.AccountBalance < order.TotalPrice)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Yêu cầu không hợp lệ!"
                };
            }
            var newOrder = new Order()
            {
                OrderId = Guid.NewGuid(),
                CustomerId = order.CustomerId,
                ServiceTypeId = order.ServiceTypeId != null ? order.ServiceTypeId.Value : null,
                DiscountId = order.DiscountId != null ? order.DiscountId.Value : null,
                CreatedDate = DateTime.Now,
                TotalPrice = order.TotalPrice,
                PartnerId = order.PartnerId,
                Status = (int)OrderStatus.WrongStatus,
            };

            string content = "";
            bool isUsingService = true;

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
                    content = ServiceTypeDefaultData.PURCHASE_PACKAGE_SERVICE_CONTENT;
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
                        var orderDetail = new OrderDetailOfBookingService()
                        {
                            OrderId = newOrder.OrderId,
                            PriceOfBookingServiceId = x.PriceOfBookingServiceId.Value,
                            Price = x.Price,
                            Content = x.Content,
                            Quantity = x.Quantity,
                            Status = 1
                        };
                        await _unitOfWork.OrderDetailOfBookingServiceRepository.Add(orderDetail);
                        content = $"Sử dụng {ServiceTypeDefaultData.BOOK_SERVICE_CONTENT}";
                        newOrder.Status = (int)OrderStatus.Unpaid; // Cập nhật lại trạng thái order đổi với service 'đặt xe' 
                    }
                    else if (serviceType.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME))
                    {
                        var orderDetail = new OrderDetailOfRentingService()
                        {
                            OrderId = newOrder.OrderId,
                            PriceOfRentingService = x.PriceOfRentingServiceId.Value,
                            Price = x.Price,
                            Content = x.Content,
                            Quantity = x.Quantity,
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


            dynamic currentPackageIsUsed = null;
            bool isUsingPackage = false;
            decimal orderPrice = 0;

            if (isUsingService) // kiểm tra có sử dụng package hay không
            {
                currentPackageIsUsed = await _packageService.GetCurrentPackageIsUsed(newOrder.CustomerId);
                if (currentPackageIsUsed != null && order.Distance != null)
                {
                    var distancesNow = currentPackageIsUsed.CurrentDistances + order.Distance;
                    var cardSwipesNow = currentPackageIsUsed.CurrentCardSwipes + 1;

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
                                newOrder.ServiceTypeId.Value == Guid.Parse(ServiceTypeDefaultData.BOOK_SERVICE_ID))
                    {
                        // write code using package for booking service
                    }
                }
            }


            // Tạo transaction khi customer sử dụng dịch vụ
            var transaction = new Transaction()
            {
                TransactionId = Guid.NewGuid(),
                Content = content,
                OrderId = newOrder.OrderId,
                CreatedDate = newOrder.CreatedDate,
                Amount = -newOrder.TotalPrice,
                Status = 1,
                WalletId = wallet.WalletId
            };

            // Cập nhật lại ví của customer
            wallet.AccountBalance -= newOrder.TotalPrice;
            await _unitOfWork.TransactionRepository.Add(transaction);
            _unitOfWork.WalletRepository.Update(wallet);
            await _unitOfWork.OrderRepository.Add(newOrder);



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
                    Content = $"Đối tác nhận 90% hóa đơn",
                    OrderId = newOrder.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount = newOrder.TotalPrice * 0.9M,
                    Status = 1,
                    WalletId = partnerWallet.WalletId
                };

                if (isUsingPackage)
                {
                    partnerTransaction.Amount = orderPrice * 0.9M;
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
                Content = $"Hệ thống nhận 10% hóa đơn",
                OrderId = newOrder.OrderId,
                CreatedDate = DateTime.Now,
                Amount = newOrder.TotalPrice * 0.1M,
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

            // Cập nhật lại ví trong trường hợp khách hàng có sử dụng dịch vụ
            if (isUsingPackage)
            {
                adminTransaction.Amount = -orderPrice * 0.9M;
                adminWallet.AccountBalance += adminTransaction.Amount;
                adminTransaction.Content = "Hệ thống chuyển tiền cho đối tác";
            }
            await _unitOfWork.TransactionRepository.Add(adminTransaction);
            _unitOfWork.WalletRepository.Update(adminWallet);

            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo hóa đơn thành công!"
            };
        }

        /* public async Task<Response> MakeOrderTest(MakeOrderTestModel model)
         {
             var newOrder = new CreateOrderModel()
             {
                 CustomerId = model.CustomerId,
                 ServiceTypeId = model.ServiceTypeId != null ? model.ServiceTypeId.Value : null,
                 DiscountId = model.DiscountId != null ? model.DiscountId.Value : null,
                 TotalPrice = model.TotalPrice
             };

             var newOrderDetailInfo = new OrderDetailsInfo()
             {
                 PackageId = model.PriceOfBusServiceId != null ? model.PackageId.Value : null,
                 PriceOfBusServiceId = model.PriceOfBusServiceId != null ? model.PriceOfBusServiceId.Value : null,
                 PriceOfBookingServiceId = model.PriceOfBookingServiceId != null ? model.PriceOfBookingServiceId.Value : null,
                 PriceOfRentingServiceId = model.PriceOfRentingServiceId != null ? model.PriceOfRentingServiceId.Value : null,
                 Price = model.Price,
                 Quantity = model.Quantity,
                 Content = model.Content,

             };

             return await CreateOrder(newOrder, newOrderDetailInfo);
         }*/
    }
}