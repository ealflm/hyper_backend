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
        Task<List<DiscountViewModel>> GetListDiscounts();
        Task<DiscountViewModel> GetDiscount(Guid id);
        Task<Response> CreateDiscount(CreateDiscountModel model);
        Task<Response> UpdateDiscount(Guid id, CreateDiscountModel model);
        Task<Response> DeleteDiscount(Guid id);
        SearchResultViewModel<DiscountViewModel> SearchDiscount(DiscountSearchModel model);
    }
}