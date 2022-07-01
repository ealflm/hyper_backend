using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class CustomerPackagesHistoryService : BaseService, ICustomerPackagesHistoryService
    {
        public CustomerPackagesHistoryService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }
        public async Task<SearchResultViewModel<CustomerPackagesHistoryViewModel>> GetCustomerPackageHistory(CustomerPackagesHistorySearchModel model)
        {
            List<Order> ordersList = await _unitOfWork.OrderRepository
                                    .Query()
                                    .Where(order => model.CustomerId == null || order.CustomerId == model.CustomerId.Value)
                                    .Where(order => model.TimeStart == null || DateTime.Compare(model.TimeStart.Value, order.CreatedDate) >= 0)
                                    .ToListAsync();

            List<CustomerPackagesHistoryViewModel> cusPacHisList = new List<CustomerPackagesHistoryViewModel>();
            foreach (Order order in ordersList)
            {
                var orderDetail = await _unitOfWork.OrderDetailOfPackageRepository
                                .Query()
                                .Where(x => x.OrderId == order.OrderId)
                                .FirstOrDefaultAsync();
                if (orderDetail != null)
                {
                    var package = await _unitOfWork.PackageRepository.GetById(orderDetail.PackageId);
                    var timeEnd = order.CreatedDate.AddDays((double)package.Duration);
                    var packageHistoryItem = new CustomerPackagesHistoryViewModel()
                    {
                        CustomerId = order.CustomerId,
                        PackageId = package.PackageId,
                        PackageName = package.Name,
                        TimeStart = order.CreatedDate,
                        TimeEnd = timeEnd,
                        Status = DateTime.Now.CompareTo(timeEnd) > 0 ? 0 : 1
                    };
                    cusPacHisList.Add(packageHistoryItem);
                }
            }
            var listAfterSorting = GetListAfterSorting(cusPacHisList, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            return new SearchResultViewModel<CustomerPackagesHistoryViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
        }
    }
}