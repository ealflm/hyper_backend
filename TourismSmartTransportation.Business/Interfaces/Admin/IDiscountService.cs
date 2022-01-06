using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IDiscountService
    {
        Task<List<DiscountViewModel>> GetListDiscounts();
        Task<DiscountViewModel> GetDiscount(Guid id);
        Task<DataModel> CreateDiscount(CreateDiscountModel model);
        Task<DataModel> UpdateDiscount(Guid id, DiscountSearchModel model);
        Task<DataModel> DeleteDiscount(Guid id);
        Task<SearchDiscountResultViewModel> SearchDiscount(DiscountSearchModel model);
    }
}