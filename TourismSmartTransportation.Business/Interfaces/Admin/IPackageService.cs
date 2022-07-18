using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPackageService
    {
        Task<SearchResultViewModel<PackageViewModel>> SearchPackage(PackageSearchModel model);
        Task<PackageViewModel> GetPackage(Guid id);
        Task<Response> CreatePackage(CreatePackageModel model);
        Task<Response> UpdatePackage(Guid id, UpdatePackageModel model);
        Task<Response> DeletePackage(Guid id);
        Task<SearchResultViewModel<PackageViewModel>> GetPackageNotUsed(PackageCustomerModel model);
    }
}