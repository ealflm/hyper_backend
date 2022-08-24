using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Admin.Package;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class PackageService : BaseService, IPackageService
    {
        private readonly ICustomerPackagesHistoryService _cusPacHisService;
        public PackageService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient,
                                ICustomerPackagesHistoryService cusPacHisService) : base(unitOfWork, blobServiceClient)
        {
            _cusPacHisService = cusPacHisService;
        }

        public async Task<Response> CreatePackage(CreatePackageModel model)
        {
            var isExisted = await _unitOfWork.PackageRepository.Query().AnyAsync(x => x.Name == model.Name);
            if (isExisted)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Gói dịch vụ đã tồn tại!"
                };
            }

            var entity = new Package()
            {
                PackageId = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                PromotedTitle = model.PromotedTitle,
                Price = model.Price,
                PhotoUrl = UploadFile(model.UploadFile, Container.Admin).Result,
                Status = 1,
                Duration = model.Duration,
                PeopleQuantity = model.PeopleQuantity
            };
            await _unitOfWork.PackageRepository.Add(entity);
            foreach (CreatePackageItemModel x in model.PackageItems)
            {
                x.PackageId = entity.PackageId;
                await _unitOfWork.PackageItemRepository.Add(x.AsPackageItemData());
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo gói dịch vụ thành công!"
            };
        }

        public async Task<Response> DeletePackage(Guid id)
        {
            var package = await _unitOfWork.PackageRepository.GetById(id);
            if (package == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            package.Status = 0;
            _unitOfWork.PackageRepository.Update(package);
            var packages = await _unitOfWork.PackageItemRepository.Query().Where(x => x.PackageId.Equals(package.PackageId)).ToListAsync();
            foreach (PackageItem x in packages)
            {
                x.Status = 0;
                _unitOfWork.PackageItemRepository.Update(x);
            }
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<PackageViewModel> GetPackage(Guid id)
        {
            var package = (await _unitOfWork.PackageRepository.GetById(id)).AsPackageViewModel();
            if (package == null)
            {
                return null;
            }
            package.PackageItems = await _unitOfWork.PackageItemRepository.Query().Where(x => x.PackageId.Equals(package.Id)).Select(x => x.AsPackageItemViewModel()).ToListAsync();
            return package;
        }

        public async Task<SearchResultViewModel<PackageViewModel>> SearchPackage(PackageSearchModel model)
        {
            var package = await _unitOfWork.PackageRepository.Query()
                            .Where(x => model.Name == null || x.Name.Contains(model.Name))
                            .Where(x => model.Description == null || x.Description.Contains(model.Description))
                            .Where(x => model.PromotedTitle == null || x.PromotedTitle.Contains(model.PromotedTitle))
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsPackageViewModel())
                            .ToListAsync();
            var listAfterSorting = GetListAfterSorting(package, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var item in listItemsAfterPaging)
            {
                var packageItems = await _unitOfWork.PackageItemRepository.Query()
                                    .Where(x => x.PackageId == item.Id)
                                    .Select(x => x.AsPackageItemViewModel())
                                    .ToListAsync();
                item.PackageItems = packageItems;
            }
            SearchResultViewModel<PackageViewModel> result = null;
            result = new SearchResultViewModel<PackageViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdatePackage(Guid id, UpdatePackageModel model)
        {
            var package = await _unitOfWork.PackageRepository.GetById(id);
            if (package == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            var isExisted = await _unitOfWork.PackageRepository.Query().AnyAsync(x => x.Name.Equals(model.Name));
            if (isExisted && model.Name != package.Name)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Gói dịch vụ đã tồn tại!"
                };
            }

            package.Name = UpdateTypeOfNullAbleObject<string>(package.Name, model.Name);
            package.Description = UpdateTypeOfNullAbleObject<string>(package.Description, model.Description);
            package.PromotedTitle = UpdateTypeOfNullAbleObject<string>(package.PromotedTitle, model.PromotedTitle);
            package.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Admin, package.PhotoUrl);
            package.PhotoUrl += await UploadFile(model.UploadFile, Container.Admin);
            package.Price = UpdateTypeOfNotNullAbleObject<decimal>(package.Price, model.Price);
            package.Status = UpdateTypeOfNotNullAbleObject<int>(package.Status, model.Status);
            package.Duration = UpdateTypeOfNotNullAbleObject<int>(package.Duration, model.Duration);
            package.PeopleQuantity = UpdateTypeOfNotNullAbleObject<int>(package.PeopleQuantity, model.PeopleQuantity);
            package.PackageItems = await _unitOfWork.PackageItemRepository
                                .Query()
                                .Where(x => x.PackageId == package.PackageId)
                                .ToListAsync();
            _unitOfWork.PackageRepository.Update(package);

            if (model.PackageItems != null && model.PackageItems.Count() > 0)
            {
                foreach (var p in package.PackageItems)
                {
                    _unitOfWork.PackageItemRepository.Remove(p);
                }

                foreach (UpdatePackageItemModel x in model.PackageItems)
                {
                    x.PackageId = package.PackageId;
                    await _unitOfWork.PackageItemRepository.Add(x.AsPackageItemData());
                }
            }
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }

        public async Task<SearchResultViewModel<PackageViewModel>> GetPackageNotUsed(PackageCustomerModel model)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(model.CustomerId);
            if (customer == null)
            {
                return null;
            }
            CustomerPackagesHistorySearchModel cusPacHisModel = new CustomerPackagesHistorySearchModel() { CustomerId = customer.CustomerId };
            var packageHisList = await _cusPacHisService.GetCustomerPackageHistory(cusPacHisModel);
            var packageIdIsUsedList = packageHisList.Items
                                        .Where(x => x.Status == 1)
                                        .Select(x => x.PackageId)
                                        .ToList();

            string sql = $@"Select * From Package Where Status = 1 ";
            List<object> paramsList = new List<object>();
            for (int i = 0; i < packageIdIsUsedList.Count; i++)
            {
                sql += "And PackageId != {" + i + "} ";
                paramsList.Add(packageIdIsUsedList[i]);
            }
            var packagesList = await _unitOfWork.PackageRepository
                                .Query()
                                .FromSqlRaw(sql, paramsList.ToArray())
                                .Select(x => x.AsPackageViewModel())
                                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(packagesList, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var item in listItemsAfterPaging)
            {
                var packageItems = await _unitOfWork.PackageItemRepository.Query()
                                    .Where(x => x.PackageId == item.Id)
                                    .Select(x => x.AsPackageItemViewModel())
                                    .ToListAsync();
                item.PackageItems = packageItems;
            }
            return new SearchResultViewModel<PackageViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
        }

        public async Task<CurrentPackageIsUsedModel> GetCurrentPackageIsUsed(Guid customerId)
        {
            List<Order> ordersList = await _unitOfWork.OrderRepository
                                    .Query()
                                    .Where(order => order.CustomerId == customerId && order.Status != (int)OrderStatus.NotUse)
                                    .Where(order => order.ServiceTypeId == Guid.Parse(ServiceTypeDefaultData.PURCHASE_PACKAGE_SERVICE_ID))
                                    .OrderByDescending(order => order.CreatedDate)
                                    .ToListAsync();

            CurrentPackageIsUsedModel result = null;

            foreach (Order order in ordersList)
            {
                var orderDetail = await _unitOfWork.OrderDetailOfPackageRepository
                                .Query()
                                .Where(x => x.OrderId == order.OrderId)
                                .FirstOrDefaultAsync();

                if (orderDetail != null)
                {
                    var currentPackages = await _unitOfWork.PackageRepository
                                    .Query()
                                    .Where(x => x.PackageId == orderDetail.PackageId)
                                    .Join(_unitOfWork.PackageItemRepository.Query(),
                                        p => p.PackageId,
                                        pi => pi.PackageId,
                                        (p, pi) => new
                                        {
                                            Package = p.AsPackageViewModel(),
                                            PackageItem = pi.AsPackageItemViewModel()
                                        }
                                    )
                                    .ToListAsync();

                    var timeEnd = order.CreatedDate.AddDays((double)currentPackages[0].Package.Duration);
                    if (DateTime.Now.CompareTo(timeEnd) < 0) // Gói dịch vụ còn hạn sử dụng
                    {
                        var customerTrips = await _unitOfWork.CustomerTripRepository
                                            .Query()
                                            .Where(x => x.CustomerId == customerId)
                                            .Where(x => x.CreatedDate.CompareTo(order.CreatedDate) >= 0 && x.CreatedDate.CompareTo(timeEnd) <= 0)
                                            .Join(_unitOfWork.VehicleRepository.Query(),
                                                customerTrip => customerTrip.VehicleId,
                                                vehicle => vehicle.VehicleId,
                                                (customerTrip, vehicle) => new { CustomerTrip = customerTrip, Vehicle = vehicle }
                                            )
                                            .Join(_unitOfWork.ServiceTypeRepository.Query(),
                                                _ => _.Vehicle.ServiceTypeId,
                                                serviceType => serviceType.ServiceTypeId,
                                                (_, serviceType) => new { CustomerTrip = _.CustomerTrip, Vehicle = _.Vehicle, ServiceType = serviceType }
                                            )
                                            .ToListAsync();

                        decimal cardSwipes = 0;
                        decimal numberOfDistances = 0;
                        decimal numberOfTrips = 0;
                        foreach (var customerTripItem in customerTrips)
                        {
                            if (customerTripItem.ServiceType.Name.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME) && customerTripItem.CustomerTrip.Distance != null)
                            {
                                cardSwipes += 1;
                                numberOfDistances += customerTripItem.CustomerTrip.Distance.Value;
                            }
                            else if (customerTripItem.ServiceType.Name.Contains(ServiceTypeDefaultData.BOOK_SERVICE_NAME))
                            {
                                numberOfTrips += 1;
                            }
                        }
                        result = new CurrentPackageIsUsedModel()
                        {
                            PackageId = currentPackages[0].Package.Id,
                            PackageName = currentPackages[0].Package.Name,
                            PackagePrice = currentPackages[0].Package.Price,
                            PackageExpire = timeEnd,
                            PackageExpireTimeStamp = ConvertDateTimeToTimestamp(timeEnd),
                            PackagePhotoUrl = currentPackages[0].Package.PhotoUrl,
                            CurrentDistances = numberOfDistances,
                            CurrentCardSwipes = cardSwipes,
                            CurrentNumberOfTrips = numberOfTrips
                        };

                        for (int i = 0; i < currentPackages.Count; i++)
                        {
                            if (currentPackages[i].PackageItem.ServiceTypeId == Guid.Parse(ServiceTypeDefaultData.BUS_SERVICE_ID))
                            {
                                result.LimitCardSwipes = currentPackages[i].PackageItem.Value;
                                result.LimitDistances = currentPackages[i].PackageItem.Limit;
                            }
                            else if (currentPackages[i].PackageItem.ServiceTypeId == Guid.Parse(ServiceTypeDefaultData.BOOK_SERVICE_ID))
                            {
                                result.LimitNumberOfTrips = currentPackages[i].PackageItem.Limit;
                                result.DiscountValueTrip = currentPackages[i].PackageItem.Value;
                            }
                        }

                        // kiểm tra xem gói dịch vụ đã xài hết chưa
                        if ((result.CurrentDistances >= result.LimitDistances || result.CurrentCardSwipes >= result.LimitCardSwipes) &&
                            result.CurrentNumberOfTrips >= result.LimitNumberOfTrips
                        )
                        {
                            result = null;
                        }

                        return result;
                    }
                }
            }

            return result;
        }

        public async Task<SearchResultViewModel<PackageViewModel>> GetAvailablePackage(PackageCustomerModel model)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(model.CustomerId);
            if (customer == null)
            {
                return null;
            }

            CustomerPackagesHistorySearchModel cusPacHisModel = new CustomerPackagesHistorySearchModel() { CustomerId = customer.CustomerId };
            var packageHisList = await _cusPacHisService.GetCustomerPackageHistory(cusPacHisModel);
            var currentPackageIsUsed = (await GetCurrentPackageIsUsed(customer.CustomerId));
            var packagesList = await _unitOfWork.PackageRepository
                                .Query()
                                .Where(CheckPackageIsUsed(currentPackageIsUsed))
                                .Where(x => x.Status == 1)
                                .Select(x => x.AsPackageViewModel())
                                .ToListAsync();

            var listAfterSorting = GetListAfterSorting(packagesList, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            foreach (var item in listItemsAfterPaging)
            {
                var packageItems = await _unitOfWork.PackageItemRepository.Query()
                                    .Where(x => x.PackageId == item.Id)
                                    .Select(x => x.AsPackageItemViewModel())
                                    .ToListAsync();
                item.PackageItems = packageItems;
            }
            return new SearchResultViewModel<PackageViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
        }

        private Expression<Func<Package, bool>> CheckPackageIsUsed(CurrentPackageIsUsedModel package = null)
        {
            if (package != null)
            {
                return x => x.PackageId != package.PackageId;
            }

            return x => true;
        }
    }
}