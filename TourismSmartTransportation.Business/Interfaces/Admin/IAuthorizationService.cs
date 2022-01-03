using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Admin.Authorization;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResultViewModel> Login(LoginSearchModel model);
        Task<AuthorizationResultViewModel> Register(RegisterSearchModel model);
    }
}
