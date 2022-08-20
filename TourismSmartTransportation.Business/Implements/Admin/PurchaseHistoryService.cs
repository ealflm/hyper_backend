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

        public async Task<SearchResultViewModel<OrderViewModel>> GetOrder(Guid customerId)
        {
            var orders = await _unitOfWork.OrderRepository.Query()
                .Where(x => x.CustomerId == customerId)
                .Select(x => x.AsOrderViewModel())
                .ToListAsync();
            foreach (var order in orders)
            {
                if (order.ServiceTypeId != null)
                {
                    order.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(order.ServiceTypeId.Value)).Name;
                }
                var customer = await _unitOfWork.CustomerRepository.GetById(order.CustomerId);
                order.CustomerName = customer.FirstName + " " + customer.LastName;
            }
            SearchResultViewModel<OrderViewModel> result = null;
            result = new SearchResultViewModel<OrderViewModel>()
            {
                Items = orders,
                PageSize = 1,
                TotalItems = orders.Count
            };
            return result;
        }

        public async Task<SearchResultViewModel<OrderViewModel>> GetOrder()
        {
            var orders = await _unitOfWork.OrderRepository.Query()
                .Select(x => x.AsOrderViewModel())
                .ToListAsync();
            foreach (var order in orders)
            {
                if (order.ServiceTypeId != null)
                {
                    order.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(order.ServiceTypeId.Value)).Name;
                }
                var customer = await _unitOfWork.CustomerRepository.GetById(order.CustomerId);
                order.CustomerName = customer.FirstName + " " + customer.LastName;
            }
            SearchResultViewModel<OrderViewModel> result = null;
            result = new SearchResultViewModel<OrderViewModel>()
            {
                Items = orders,
                PageSize = 1,
                TotalItems = orders.Count
            };
            return result;
        }

        private async Task<SearchResultViewModel<OrderDetailOfPackageViewModel>> GetOrderDetailOfPackage(Guid orderId)
        {
            var orderDetailList = await _unitOfWork.OrderDetailOfPackageRepository.Query()
                .Where(x => x.OrderId == orderId)
                .Select(x => x.AsOrderDetailOfPackageViewModel())
                .ToListAsync();
            SearchResultViewModel<OrderDetailOfPackageViewModel> result = null;
            result = new SearchResultViewModel<OrderDetailOfPackageViewModel>()
            {
                Items = orderDetailList,
                PageSize = 1,
                TotalItems = orderDetailList.Count
            };
            return result;
        }

        private async Task<SearchResultViewModel<OrderDetailOfBusServiceViewModel>> GetOrderDetailOfBusService(Guid orderId)
        {
            var orderDetailList = await _unitOfWork.OrderDetailOfBusServiceRepository.Query()
                .Where(x => x.OrderId == orderId)
                .Select(x => x.AsOrderDetailOfBusServiceViewModel())
                .ToListAsync();
            SearchResultViewModel<OrderDetailOfBusServiceViewModel> result = null;
            result = new SearchResultViewModel<OrderDetailOfBusServiceViewModel>()
            {
                Items = orderDetailList,
                PageSize = 1,
                TotalItems = orderDetailList.Count
            };
            return result;
        }

        private async Task<SearchResultViewModel<OrderDetailOfBookingServiceViewModel>> GetOrderDetailOfBookingService(Guid orderId)
        {
            var orderDetailList = await _unitOfWork.OrderDetailOfBookingServiceRepository.Query()
                .Where(x => x.OrderId == orderId)
                .Select(x => x.AsOrderDetailOfBookingServiceViewModel())
                .ToListAsync();
            SearchResultViewModel<OrderDetailOfBookingServiceViewModel> result = null;
            result = new SearchResultViewModel<OrderDetailOfBookingServiceViewModel>()
            {
                Items = orderDetailList,
                PageSize = 1,
                TotalItems = orderDetailList.Count
            };
            return result;
        }

        private async Task<SearchResultViewModel<OrderDetailOfRentingServiceViewModel>> GetOrderDetailOfRentingService(Guid orderId)
        {
            var orderDetailList = await _unitOfWork.OrderDetailOfRentingServiceRepository.Query()
                .Where(x => x.OrderId == orderId)
                .Select(x => x.AsOrderDetailOfRentingServiceViewModel())
                .ToListAsync();
            SearchResultViewModel<OrderDetailOfRentingServiceViewModel> result = null;
            result = new SearchResultViewModel<OrderDetailOfRentingServiceViewModel>()
            {
                Items = orderDetailList,
                PageSize = 1,
                TotalItems = orderDetailList.Count
            };
            return result;
        }

        public async Task<Object> GetOrderDetail(Guid orderId)
        {
            var package = await GetOrderDetailOfPackage(orderId);
            if (package != null && package.Items.Count > 0)
            {
                return package;
            }

            var bus = await GetOrderDetailOfBusService(orderId);
            if (bus != null && bus.Items.Count > 0)
            {
                return bus;
            }

            var booking = await GetOrderDetailOfBookingService(orderId);
            if (booking != null && booking.Items.Count > 0)
            {
                return booking;
            }

            var renting = await GetOrderDetailOfRentingService(orderId);
            if (renting != null && renting.Items.Count > 0)
            {
                return renting;
            }

            return null;
        }

        public async Task<SearchResultViewModel<TransactionViewModel>> GetTransaction(Guid orderId)
        {
            var transationsList = await _unitOfWork.TransactionRepository.Query()
                .Where(x => x.OrderId == orderId && x.Status==1)
                .Join(_unitOfWork.OrderRepository.Query(),
                    transaction => transaction.OrderId,
                    order => order.OrderId,
                    (transaction, order) => new { transaction, order }
                )
                .Join(_unitOfWork.CustomerRepository.Query(),
                    trans_ord => trans_ord.order.CustomerId,
                    customer => customer.CustomerId,
                    (trans_ord, customer) => new TransactionViewModel()
                    {
                        OrderId = trans_ord.order.OrderId,
                        Amount = trans_ord.transaction.Amount,
                        CreatedDate = trans_ord.transaction.CreatedDate,
                        Content = trans_ord.transaction.Content,
                        Status = trans_ord.transaction.Status,
                        CustomerName = customer.LastName + " " + customer.FirstName,
                        PartnerId = trans_ord.order.PartnerId
                    }
                )
                // .Select(x => x.AsTransactionViewModel())
                .ToListAsync();

            var pairs = new List<Tuple<Guid, string>>();
            foreach (var item in transationsList)
            {
                if (pairs.Count == 0 && item.PartnerId != null)
                {
                    var partner = await _unitOfWork.PartnerRepository.GetById(item.PartnerId.Value);
                    item.CompanyName = partner.CompanyName;
                    pairs.Add(Tuple.Create(partner.PartnerId, partner.CompanyName));
                    continue;
                }

                if (item.PartnerId != null)
                {
                    foreach (var p in pairs)
                    {
                        if (p.Item1 == item.PartnerId.Value)
                        {
                            item.CompanyName = p.Item2;
                            break;
                        }
                    }
                    var partner = await _unitOfWork.PartnerRepository.GetById(item.PartnerId.Value);
                    item.CompanyName = partner.CompanyName;
                    pairs.Add(Tuple.Create(partner.PartnerId, partner.CompanyName));
                }
            }
            SearchResultViewModel<TransactionViewModel> result = null;
            result = new SearchResultViewModel<TransactionViewModel>()
            {
                Items = transationsList,
                PageSize = 1,
                TotalItems = transationsList.Count
            };
            return result;
        }

        public async Task<SearchResultViewModel<OrderViewModel>> GetOrderByPartnerId(Guid partnerId)
        {
            var orders = await _unitOfWork.OrderRepository.Query()
                .Where(x => x.PartnerId.Equals(partnerId))
                .Select(x => x.AsOrderViewModel())
                .ToListAsync();
            foreach (var order in orders)
            {
                if (order.ServiceTypeId != null)
                {
                    order.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(order.ServiceTypeId.Value)).Name;
                }
                var customer = await _unitOfWork.CustomerRepository.GetById(order.CustomerId);
                order.CustomerName = customer.FirstName + " " + customer.LastName;
            }
            SearchResultViewModel<OrderViewModel> result = null;
            result = new SearchResultViewModel<OrderViewModel>()
            {
                Items = orders,
                PageSize = 1,
                TotalItems = orders.Count
            };
            return result;
        }
    }
}
