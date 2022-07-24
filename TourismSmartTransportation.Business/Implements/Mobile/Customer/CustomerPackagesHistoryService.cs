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

        public async Task<List<PackageDetailsViewModel>> GetPackageDetails(PackageDetailsSearchModel model)
        {
            var customerTripsList = await _unitOfWork.CustomerTripRepository
                                .Query()
                                .Where(x => x.CustomerId == model.CustomerId)
                                .Select(x => x.AsCustomerTripViewModel())
                                .ToListAsync();

            var pairs = new List<Tuple<Guid, decimal?>>();
            foreach (var p in customerTripsList)
            {
                if (pairs.Count == 0)
                {
                    var serviceType = (await _unitOfWork.VehicleRepository
                                    .Query()
                                    .Where(x => x.VehicleId == p.VehicleId)
                                    .Select(x => x.AsVehicleViewModel())
                                    .FirstOrDefaultAsync()).ServiceTypeId;
                    pairs.Add(Tuple.Create(serviceType, p.Distance));
                }
                else
                {
                    bool check = false;
                    for (int i = 0; i < pairs.Count; i++)
                    {
                        if (pairs[i].Item1 == p.VehicleId)
                        {
                            var item1 = pairs[i].Item1;
                            var item2 = pairs[i].Item2 + p.Distance;
                            pairs.RemoveAt(i);
                            pairs.Add(Tuple.Create(item1, item2));
                            check = true;
                            break;
                        }
                    }

                    if (!check)
                    {
                        var serviceType = (await _unitOfWork.VehicleRepository
                                    .Query()
                                    .Where(x => x.VehicleId == p.VehicleId)
                                    .Select(x => x.AsVehicleViewModel())
                                    .FirstOrDefaultAsync()).ServiceTypeId;
                        pairs.Add(Tuple.Create(serviceType, p.Distance));
                    }
                }
            }

            var packageItemsList = await _unitOfWork.PackageItemRepository
                            .Query()
                            .Where(x => x.PackageId == model.PackageId)
                            .Select(x => x.AsPackageItemViewModel())
                            .ToListAsync();

            List<PackageDetailsViewModel> result = new List<PackageDetailsViewModel>();

            foreach (var p in packageItemsList)
            {
                for (int i = 0; i < pairs.Count; i++)
                {
                    if (p.ServiceTypeId == pairs[i].Item1)
                    {
                        result.Add(new()
                        {
                            ServiceTypeId = p.ServiceTypeId,
                            ServiceName = (await _unitOfWork.ServiceTypeRepository.GetById(p.ServiceTypeId)).Name,
                            LimitValue = p.Limit,
                            CurrentValue = pairs[i].Item2.Value,
                            DiscountValue = p.Value
                        });
                        pairs.RemoveAt(i);
                        break;
                    }
                }
            }

            return result;
        }
    }
}