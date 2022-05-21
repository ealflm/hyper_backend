using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IDiscountService
    {
        Task<DiscountViewModel> GetDiscount(Guid id);
        Task<bool> CreateDiscount(CreateDiscountModel model);
        Task<bool> UpdateDiscount(Guid id, UpdateDiscountModel model);
        Task<bool> DeleteDiscount(Guid id);
        Task<SearchResultViewModel<DiscountViewModel>> SearchDiscount(DiscountSearchModel model);
    }
}