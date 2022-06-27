using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PurchaseHistoryService : BaseService, IPurchaseHistoryService
    {
        public PurchaseHistoryService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<SearchResultViewModel<OrderViewModel>> GetOrder(string customerId)
        {
            var orders = await _unitOfWork.OrderRepository.Query()
                .Where(x => customerId != null && x.CustomerId.Equals(new Guid(customerId)))
                .Select(x => x.AsOrderViewModel())
                .ToListAsync();
            SearchResultViewModel<OrderViewModel> result = null;
            result = new SearchResultViewModel<OrderViewModel>()
            {
                Items = orders,
                PageSize = 1,
                TotalItems = orders.Count
            };
            return result;
        }

        public async Task<SearchResultViewModel<OrderDetailViewModel>> GetOrderDetail(string orderId)
        {
            var orderDetailList = await _unitOfWork.OrderDetailRepository.Query()
                .Where(x => orderId != null && x.OrderId.Equals(new Guid(orderId)))
                .Select(x => x.AsOrderDetailViewModel())
                .ToListAsync();
            SearchResultViewModel<OrderDetailViewModel> result = null;
            result = new SearchResultViewModel<OrderDetailViewModel>()
            {
                Items = orderDetailList,
                PageSize = 1,
                TotalItems = orderDetailList.Count
            };
            return result;
        }

        public async Task<SearchResultViewModel<PaymentViewModel>> GetPayment(string orderId)
        {
            var paymentList = await _unitOfWork.PaymentRepository.Query()
                .Where(x => orderId != null && x.OrderId.Equals(new Guid(orderId)))
                .Select(x => x.AsPaymentViewModel())
                .ToListAsync();
            SearchResultViewModel<PaymentViewModel> result = null;
            result = new SearchResultViewModel<PaymentViewModel>()
            {
                Items = paymentList,
                PageSize = 1,
                TotalItems = paymentList.Count
            };
            return result;
        }

    }
}
