// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Azure.Storage.Blobs;
// using Microsoft.EntityFrameworkCore;
// using TourismSmartTransportation.Business.Extensions;
// using TourismSmartTransportation.Business.Interfaces.Shared;
// using TourismSmartTransportation.Business.SearchModel.Shared;
// using TourismSmartTransportation.Business.ViewModel.Common;
// using TourismSmartTransportation.Business.ViewModel.Shared;
// using TourismSmartTransportation.Data.Interfaces;
// using TourismSmartTransportation.Data.Models;

// namespace TourismSmartTransportation.Business.Implements.Shared
// {
//     public class CustomerTierHistoryService : BaseService, ICustomerTierHistoryService
//     {
//         public CustomerTierHistoryService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
//         {
//         }
//         public async Task<SearchResultViewModel<CustomerTierHistoryViewModel>> Get(CustomerTierHistorySearchModel model)
//         {
//             List<Order> entity = null;
//             try
//             {
//                 entity = await _unitOfWork.OrderRepository.Query()
//                             .Where(x => model.CustomerId == null || x.CustomerId == model.CustomerId.Value)
//                             .Where(x => model.TimeStart == null || DateTime.Compare(model.TimeStart.Value, x.CreatedDate) >= 0)
//                             .Where(x => model.TimeEnd == null || DateTime.Compare(model.TimeEnd.Value, x.CreatedDate) < 0)
//                             .Where(x => model.Status == null || x.Status == model.Status.Value)
//                             .ToListAsync();
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e.Message);
//             }
//             List<CustomerTierHistoryViewModel> customerTierHistoryViewModels = new List<CustomerTierHistoryViewModel>();
//             foreach (Order x in entity)
//             {
//                 var orderDetail = await _unitOfWork.OrderDetailRepository.Query().Where(y => y.TierId != null && y.OrderId.Equals(x.Id)).ToListAsync();
//                 if (orderDetail.Count != 0)
//                 {
//                     var tier = await _unitOfWork.PackageRepository.GetById(orderDetail[0].TierId.Value);
//                     var item = new CustomerTierHistoryViewModel();
//                     item.Id = x.Id;
//                     item.CustomerId = x.CustomerId;
//                     item.TierId = orderDetail[0].TierId.Value;
//                     item.TierName = tier.Name;
//                     item.TimeStart = x.CreatedDate;
//                     item.TimeEnd = x.CreatedDate.AddDays((double)tier.Duration);
//                     item.Status = DateTime.Now.CompareTo(item.TimeEnd) > 0 ? 0 : 1;
//                     customerTierHistoryViewModels.Add(item);
//                 }
//             }
//             var listAfterSorting = GetListAfterSorting(customerTierHistoryViewModels, model.SortBy);
//             var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
//             var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
//             SearchResultViewModel<CustomerTierHistoryViewModel> result = null;
//             result = new SearchResultViewModel<CustomerTierHistoryViewModel>()
//             {
//                 Items = listItemsAfterPaging,
//                 PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
//                 TotalItems = totalRecord
//             };
//             return result;
//         }
//     }
// }