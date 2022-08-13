using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class BuyPackageService : BaseService, IBuyPackageService
    {
        private readonly IOrderHelpersService _orderHelpers;
        private readonly IPackageService _packageService;

        public BuyPackageService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IOrderHelpersService orderHelpers,
                                    IPackageService packageService) : base(unitOfWork, blobServiceClient)
        {
            _orderHelpers = orderHelpers;
            _packageService = packageService;
        }

        public async Task<Response> BuyPackage(BuyPackageModel model)
        {
            var currentPackageIsUsed = await _packageService.GetCurrentPackageIsUsed(model.CustomerId);
            if (currentPackageIsUsed != null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Khách hàng không thể mua thêm gói dịch vụ!"
                };
            }

            var package = await _unitOfWork.PackageRepository.GetById(model.PackageId);
            if (package == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Gói dịch vụ không hợp lệ!"
                };
            }

            OrderDetailsInfo orderDetails = new OrderDetailsInfo()
            {
                PackageId = package.PackageId,
                Content = ServiceTypeDefaultData.PURCHASE_PACKAGE_SERVICE_CONTENT,
                Price = package.Price,
                Quantity = 1,
            };
            var orderDetailList = new List<OrderDetailsInfo>();
            orderDetailList.Add(orderDetails);

            var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Contains(ServiceTypeDefaultData.PURCHASE_PACKAGE_SERVICE_NAME)).FirstOrDefaultAsync();
            if (serviceType == null)
            {

                return new()
                {
                    StatusCode = 500,
                    Message = "Không tìm thấy loại dịch vụ!"
                };
            }

            CreateOrderModel createOrder = new CreateOrderModel()
            {
                CustomerId = model.CustomerId,
                ServiceTypeId = serviceType.ServiceTypeId,
                TotalPrice = package.Price,
                OrderDetailsInfos = orderDetailList
            };
            var respone = await _orderHelpers.CreateOrder(createOrder);
            if (respone.StatusCode != 201)
            {
                return respone;
            }

            return new()
            {
                StatusCode = 201,
                Message = "Mua gói dịch vụ thành công"
            };
        }
    }
}