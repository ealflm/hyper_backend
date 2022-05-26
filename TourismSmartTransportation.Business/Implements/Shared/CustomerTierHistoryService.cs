using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Shared;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class CustomerTierHistoryService : BaseService, ICustomerTierHistoryService
    {
        public CustomerTierHistoryService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }
        public async Task<SearchResultViewModel<CustomerTierHistoryViewModel>> Get(CustomerTierHistorySearchModel model)
        {
            var entity = await _unitOfWork.CustomerTierHistoryRepository.Query()
                        .Where(x => model.CustomerId == null || x.CustomerId == model.CustomerId)
                        .Where(x => model.TierId == null || x.TierId == model.TierId)
                        .Where(x => model.TimeStart == null || DateTime.Compare(model.TimeStart.Value, x.TimeStart) >= 0)
                        .Where(x => model.TimeEnd == null || DateTime.Compare(x.TimeEnd, model.TimeEnd.Value) <= 0)
                        .Where(x => model.Status == null || x.Status == model.Status.Value)
                        .Select(x => x.AsCustomerTierHistoryViewModel())
                        .ToListAsync();
            foreach(CustomerTierHistoryViewModel x in entity)
            {
                x.TierName = (await _unitOfWork.TierRepository.GetById(x.TierId)).Name;
            }
            var listAfterSorting = GetListAfterSorting(entity, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<CustomerTierHistoryViewModel> result = null;
            result = new SearchResultViewModel<CustomerTierHistoryViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }
    }
}