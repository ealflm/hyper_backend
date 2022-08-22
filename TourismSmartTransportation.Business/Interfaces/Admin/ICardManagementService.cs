using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.CardManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CardManagement;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface ICardManagementService
    {
        Task<List<CardViewModel>> Search(CardSearchModel model);
        Task<CardViewModel> GetById(Guid id);
        Task<CardViewModel> GetByCustomerId(Guid customerId);
        Task<Response> Create(string uid);
        Task<Response> Update(Guid id, UpdateCardModel model);
        Task<Response> Delete(Guid id);
        Task<Response> CustomerMatch(UpdateCardModel model);
    }
}