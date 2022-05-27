using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Tier;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Admin.Tier;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class TierService : BaseService, ITierService
    {
        public TierService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreateTier(CreateTierModel model)
        {
            var isExisted = await _unitOfWork.TierRepository.Query().AnyAsync(x => x.Name == model.Name);
            if (isExisted)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Tier đã tồn tại!"
                };
            }

            var entity = new Tier()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                PromotedTitle = model.PromotedTitle,
                Price = model.Price,
                PhotoUrl = UploadFile(model.UploadFile, Container.Admin).Result,
                Status = 1,
            };
            await _unitOfWork.TierRepository.Add(entity);
            foreach (CreatePackageModel x in model.PackageList)
            {
                x.TierId = entity.Id;
                await _unitOfWork.PackageRepository.Add(x.AsPackageData());
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo Tier thành công!"
            };
        }

        public async Task<Response> DeleteTier(Guid id)
        {
            var tier = await _unitOfWork.TierRepository.GetById(id);
            if (tier == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            tier.Status = 0;
            _unitOfWork.TierRepository.Update(tier);
            var packages = await _unitOfWork.PackageRepository.Query().Where(x => x.TierId.Equals(tier.Id)).ToListAsync();
            foreach (Package x in packages)
            {
                x.Status = 0;
                _unitOfWork.PackageRepository.Update(x);
            }
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<TierViewModel> GetTier(Guid id)
        {
            var tier = (await _unitOfWork.TierRepository.GetById(id)).AsTierViewModel();
            if (tier == null)
            {
                return null;
            }
            tier.PackageList = await _unitOfWork.PackageRepository.Query().Where(x => x.TierId.Equals(tier.Id)).Select(x => x.AsPackageViewModel()).ToListAsync();
            return tier;
        }

        public async Task<SearchResultViewModel<TierViewModel>> SearchTier(TierSearchModel model)
        {
            var tier = await _unitOfWork.TierRepository.Query()
                            .Where(x => model.Name == null || x.Name.Equals(model.Name))
                            .Where(x => model.Description == null || x.Description.Contains(model.Description))
                            .Where(x => model.PromotedTitle == null || x.PromotedTitle.Contains(model.PromotedTitle))
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsTierViewModel())
                            .ToListAsync();
            var listAfterSorting = GetListAfterSorting(tier, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var item in listItemsAfterPaging)
            {
                var packageList = await _unitOfWork.PackageRepository.Query()
                                    .Where(x => x.TierId == item.Id)
                                    .Select(x => x.AsPackageViewModel())
                                    .ToListAsync();
                item.PackageList = packageList;
            }
            SearchResultViewModel<TierViewModel> result = null;
            result = new SearchResultViewModel<TierViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdateTier(Guid id, UpdateTierModel model)
        {
            var tier = await _unitOfWork.TierRepository.GetById(id);
            if (tier == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            var isExisted = await _unitOfWork.TierRepository.Query().AnyAsync(x => x.Name.Equals(model.Name));
            if (isExisted && model.Name != tier.Name)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Tier đã tồn tại!"
                };
            }

            tier.Name = UpdateTypeOfNullAbleObject<string>(tier.Name, model.Name);
            tier.Description = UpdateTypeOfNullAbleObject<string>(tier.Description, model.Description);
            tier.PromotedTitle = UpdateTypeOfNullAbleObject<string>(tier.PromotedTitle, model.PromotedTitle);
            tier.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Admin, tier.PhotoUrl);
            tier.PhotoUrl += await UploadFile(model.UploadFile, Container.Admin);
            tier.Price = UpdateTypeOfNotNullAbleObject<decimal>(tier.Price, model.Price);
            tier.Status = UpdateTypeOfNotNullAbleObject<int>(tier.Status, model.Status);
            _unitOfWork.TierRepository.Update(tier);
            if (model.PackageList != null && model.PackageList.Count() > 0)
            {
                foreach (UpdatePackageModel x in model.PackageList)
                {
                    _unitOfWork.PackageRepository.Update(x.AsPackageData());
                }
            }
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}