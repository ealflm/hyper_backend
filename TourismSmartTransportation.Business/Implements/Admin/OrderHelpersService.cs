using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
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
        public readonly string Bus = "Đi xe theo chuyến";
        public readonly string Booking = "Đặt xe";
        public readonly string Renting = "Thuê xe";

        public OrderHelpersService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        /// <summary>
        /// Create a order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response> CreateOrder(CreateOrderModel order, OrderDetailsInfo orderDetailsInfo)
        {
            var newOrder = new Order()
            {
                OrderId = Guid.NewGuid(),
                CustomerId = order.CustomerId,
                ServiceTypeId = order.ServiceTypeId != null ? order.ServiceTypeId.Value : null,
                DiscountId = order.DiscountId != null ? order.DiscountId.Value : null,
                CreatedDate = DateTime.Now,
                TotalPrice = order.TotalPrice,
                Status = 1,
            };

            // Customer buy package
            if (orderDetailsInfo.PackageId != null)
            {
                var orderDetail = new OrderDetailOfPackage()
                {
                    OrderId = newOrder.OrderId,
                    PackageId = orderDetailsInfo.PackageId.Value,
                    Price = orderDetailsInfo.Price,
                    Content = orderDetailsInfo.Content,
                    Quantity = orderDetailsInfo.Quantity,
                    Status = 1
                };
                await _unitOfWork.OrderDetailOfPackageRepository.Add(orderDetail);
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

                if (serviceType.Name.Contains(Bus))
                {
                    var orderDetail = new OrderDetailOfBusService()
                    {
                        OrderId = newOrder.OrderId,
                        PriceOfBusServiceId = orderDetailsInfo.PriceOfBusServiceId.Value,
                        Price = orderDetailsInfo.Price,
                        Content = orderDetailsInfo.Content,
                        Quantity = orderDetailsInfo.Quantity,
                        Status = 1
                    };
                    await _unitOfWork.OrderDetailOfBusServiceRepository.Add(orderDetail);
                }
                else if (serviceType.Name.Contains(Booking))
                {
                    var orderDetail = new OrderDetailOfBookingService()
                    {
                        OrderId = newOrder.OrderId,
                        PriceOfBookingServiceId = orderDetailsInfo.PriceOfBusServiceId.Value,
                        Price = orderDetailsInfo.Price,
                        Content = orderDetailsInfo.Content,
                        Quantity = orderDetailsInfo.Quantity,
                        Status = 1
                    };
                    await _unitOfWork.OrderDetailOfBookingServiceRepository.Add(orderDetail);
                }
                else if (serviceType.Name.Contains(Renting))
                {
                    var orderDetail = new OrderDetailOfRentingService()
                    {
                        OrderId = newOrder.OrderId,
                        PriceOfRentingService = orderDetailsInfo.PriceOfBusServiceId.Value,
                        Price = orderDetailsInfo.Price,
                        Content = orderDetailsInfo.Content,
                        Quantity = orderDetailsInfo.Quantity,
                        Status = 1
                    };
                    await _unitOfWork.OrderDetailOfRentingServiceRepository.Add(orderDetail);
                }
                else
                {
                    // more other service type 
                }
            }

            await _unitOfWork.OrderRepository.Add(newOrder);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo hóa đơn thành công!"
            };
        }

        public async Task<Response> MakeOrderTest(MakeOrderTestModel model)
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
                PackageId = model.PackageId.Value,
                PriceOfBusServiceId = model.PriceOfBusServiceId != null ? model.PriceOfBusServiceId.Value : null,
                PriceOfBookingServiceId = model.PriceOfBookingServiceId != null ? model.PriceOfBookingServiceId.Value : null,
                PriceOfRentingServiceId = model.PriceOfRentingServiceId != null ? model.PriceOfRentingServiceId.Value : null,
                Price = model.Price,
                Quantity = model.Quantity,
                Content = model.Content,

            };

            return await CreateOrder(newOrder, newOrderDetailInfo);
        }
    }
}