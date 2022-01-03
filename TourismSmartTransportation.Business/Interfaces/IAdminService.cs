using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel;

namespace TourismSmartTransportation.Business.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminViewModel>> GetAdmin();
    }
}
