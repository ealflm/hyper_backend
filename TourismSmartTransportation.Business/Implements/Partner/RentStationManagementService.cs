using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Implements.Company
{
    public class RentStationManagementService : BaseService, IRentStationManagementService
    {
        public RentStationManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> AddRentStation(AddRentStationModel model)
        {
            var rentStation = new RentStation()
            {
                RentStationId = Guid.NewGuid(),
                PartnerId = model.PartnerId,
                Title = model.Title,
                Description = model.Description,
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Status = 1
            };
            await _unitOfWork.RentStationRepository.Add(rentStation);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo trạm thuê xe thành công!"
            };
        }

        public async Task<Response> DeleteRentStation(Guid id)
        {
            var rentStation = await _unitOfWork.RentStationRepository.GetById(id);
            if (rentStation == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }
            rentStation.Status = 0;
            _unitOfWork.RentStationRepository.Update(rentStation);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<RentStationViewModel> GetRentStation(Guid id)
        {
            var rentStation = await _unitOfWork.RentStationRepository.GetById(id);
            if (rentStation == null)
            {
                return null;
            }
            RentStationViewModel model = rentStation.AsRentStationViewModel();
            model.companyName = (await _unitOfWork.PartnerRepository.GetById(model.PartnerId)).CompanyName;
            return model;
        }

        // public async Task<SearchResultViewModel<RentStationViewModel>> SearchRentStation(RentStationSearchModel model)
        // {
        //     Func<object, SearchResultViewModel<RentStationViewModel>> returnFunc = (param) =>
        //     {
        //         RentStationSearchModel model = (RentStationSearchModel)param;
        //         var source = _unitOfWork.RentStationRepository
        //                     .FindAsNoTracking()
        //                     .FilterFunc(model);
        //         var totalItems = source.Count();
        //         var items = source
        //                         .OrderByCustomFunc(model.SortBy)
        //                         .PaginateFunc(model.PageIndex, model.ItemsPerPage)
        //                         .Select(item => item.AsRentStationViewModel())
        //                         .ToList();
        //         foreach(RentStationViewModel x in items)
        //         {
        //             x.companyName= ( _unitOfWork.PartnerRepository.GetById(x.PartnerId)).Result.CompanyName;
        //         }
        //         var pageSize = GetPageSize(model.ItemsPerPage, totalItems);
        //         return new SearchResultViewModel<RentStationViewModel>(items, pageSize, totalItems);
        //     };

        //     Task<SearchResultViewModel<RentStationViewModel>> task = new Task<SearchResultViewModel<RentStationViewModel>>(returnFunc, model);
        //     task.Start();
        //     return await task;
        // }

        public async Task<SearchResultViewModel<RentStationViewModel>> SearchRentStation(RentStationSearchModel model)
        {
            var discount = await _unitOfWork.RentStationRepository.Query()
                .Where(x => model.PartnerId == null || x.PartnerId.ToString().Contains(model.PartnerId.Value.ToString()))
                .Where(x => model.Title == null || x.Title.Contains(model.Title))
                .Where(x => model.Address == null || x.Address.Contains(model.Address))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.ModifiedDate)
                .Select(x => x.AsRentStationViewModel())
                .ToListAsync();
            foreach(RentStationViewModel x in discount)
            {
                x.TotalVehicle = await _unitOfWork.VehicleRepository.Query().Where(y => y.RentStationId.Equals(x.Id)).CountAsync();
            }
            var listAfterSorting = GetListAfterSorting(discount, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var p in listItemsAfterPaging)
            {
                p.companyName = (await _unitOfWork.PartnerRepository.GetById(p.PartnerId)).CompanyName;
            }
            SearchResultViewModel<RentStationViewModel> result = null;
            result = new SearchResultViewModel<RentStationViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdateRentStaion(Guid id, UpdateRentStation model)
        {
            var rentStation = await _unitOfWork.RentStationRepository.GetById(id);
            if (rentStation == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            rentStation.PartnerId = UpdateTypeOfNotNullAbleObject<Guid>(rentStation.PartnerId, model.PartnerId);
            rentStation.Title = UpdateTypeOfNullAbleObject<string>(rentStation.Title, model.Title);
            rentStation.Address = UpdateTypeOfNullAbleObject<string>(rentStation.Address, model.Address);
            rentStation.Description = UpdateTypeOfNullAbleObject<string>(rentStation.Description, model.Description);
            rentStation.Latitude = UpdateTypeOfNotNullAbleObject<decimal>(rentStation.Latitude, model.Latitude);
            rentStation.Longitude = UpdateTypeOfNotNullAbleObject<decimal>(rentStation.Longitude, model.Longitude);
            rentStation.ModifiedDate = DateTime.Now;
            rentStation.Status = UpdateTypeOfNotNullAbleObject<int>(rentStation.Status, model.Status);
            _unitOfWork.RentStationRepository.Update(rentStation);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}
