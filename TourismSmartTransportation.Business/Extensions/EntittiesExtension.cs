using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
//using TourismSmartTransportation.Business.ViewModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.SearchModel.Admin.ServiceType;
using TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement;
using TourismSmartTransportation.Business.ViewModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Admin.Tier;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using System;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;

namespace TourismSmartTransportation.Business.Extensions
{
    public static class EntitiesExtension
    {


        public static DiscountViewModel AsDiscountViewModel(this Discount item)
        {
            return new()
            {
                Id = item.Id,
                Title = item.Title,
                Code = item.Code,
                Description = item.Description,
                TimeStart = item.TimeStart,
                TimeEnd = item.TimeEnd,
                PhotoUrl = item.PhotoUrl,
                Value = item.Value,
                Status = item.Status
            };
        }

        public static VehicleTypeViewModel AsVehicleTypeViewModel(this VehicleType item)
        {
            return new()
            {
                Id = item.Id,
                Label = item.Label,
                Fuel = item.Fuel,
                Seats = item.Seats,
                Price = item.Price,
                Status = item.Status
            };
        }

        public static PartnerViewModel AsPartnerViewModel(this Partner item)
        {
            return new()
            {
                Id = item.Id,
                Username = item.Username,
                FirstName = item.FirstName,
                LastName = item.LastName,
                CompanyName = item.CompanyName,
                PhotoUrl = item.PhotoUrl,
                Gender = item.Gender,
                Phone = item.Phone,
                Address1 = item.Address1,
                Address2 = item.Address2,
                Email = item.Email,
                DateOfBirth = item.DateOfBirth != null ? item.DateOfBirth.Value : null,
                CreatedDate = item.CreatedDate != null ? item.CreatedDate.Value : null,
                ModifiedDate = item.ModifiedDate != null ? item.ModifiedDate.Value : null,
                Status = item.Status
            };
        }

        public static StationViewModel AsStationViewModel(this Station item)
        {
            return new()
            {
                Id = item.Id,
                Title = item.Title,
                Address = item.Address,
                Description = item.Description,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Status = item.Status
            };
        }

        public static CustomerViewModel AsCustomerViewModel(this Customer item)
        {
            return new()
            {
                Id = item.Id,
                TierId = item.TierId,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Gender = item.Gender,
                Phone = item.Phone,
                Address1 = item.Address1,
                Address2 = item.Address2,
                DateOfBirth = item.DateOfBirth != null ? item.DateOfBirth.Value : null,
                PhotoUrl = item.PhotoUrl,
                Email = item.Email,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                Status = item.Status
            };
        }

        public static RentStationViewModel AsRentStationViewModel(this RentStation item)
        {
            return new()
            {
                Id = item.Id,
                Title = item.Title,
                Address = item.Address,
                PartnerId = item.PartnerId,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Status = item.Status
            };
        }

        public static OrderViewModel AsOrderViewModel(this Order item)
        {
            return new()
            {
                Id = item.Id,
                CreatedDate = item.CreatedDate,
                CustomerId = item.CustomerId,
                TotalPrice = item.TotalPrice,
                Status = item.Status
            };
        }
        public static OrderDetailViewModel AsOrderDetailViewModel(this OrderDetail item)
        {
            return new()
            {
                Id = item.Id,
                Content = item.Content,
                OrderId = item.OrderId,
                Price = item.Price,
                Quantity = item.Quantity,
                TierId = item.TierId.GetValueOrDefault(),
                Status = item.Status
            };
        }

        public static PaymentViewModel AsPaymentViewModel(this Payment item)
        {
            return new()
            {
                Id = item.Id,
                Content = item.Content,
                Amount = item.Amount,
                CreatedDate = item.CreatedDate,
                OrderId = item.OrderId,
                Status = item.Status
            };
        }

        public static TransactionViewModel AsTransactionViewModel(this Transaction item)
        {
            return new()
            {
                Id = item.Id,
                Content = item.Content,
                Amount = item.Amount,
                CreatedDate = item.CreatedDate,
                PaymentId = item.PaymentId,
                WalletId = item.WalletId,
                Status = item.Status
            };
        }

        public static CustomerTierHistoryViewModel AsCustomerTierHistoryViewModel(this CustomerTierHistory item)
        {
            return new()
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                TierId = item.TierId,
                TimeStart = item.TimeStart,
                TimeEnd = item.TimeEnd,
                Status = item.Status
            };
        }

        public static ServiceType AsServiceTypeDataModel(this ServiceTypeSearchModel item)
        {
            return new()
            {
                Content = item.Content,
                Name = item.Name
            };
        }

        public static ServiceTypeViewModel AsServiceTypeViewModel(this ServiceType item)
        {
            return new()
            {
                Id = item.Id,
                Content = item.Content,
                Name = item.Name,
                Status = item.Status
            };
        }

        public static TierViewModel AsTierViewModel(this Tier item)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PhotoUrl = item.PhotoUrl,
                PromotedTitle = item.PromotedTitle,
                Price = item.Price,
                Status = item.Status
            };
        }

        public static PackageViewModel AsPackageViewModel(this Package item)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                Limit = item.Limit,
                ServiceTypeId = item.ServiceTypeId,
                TierId = item.TierId,
                Value = item.Value,
                Status = item.Status
            };
        }

        public static Package AsPackageData(this CreatePackageModel item)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Limit = item.Limit,
                ServiceTypeId = item.ServiceTypeId.Value,
                TierId = item.TierId.Value,
                Value = item.Value,
                Status = item.Status.Value
            };
        }

        public static Package AsPackageData(this UpdatePackageModel item)
        {
            return new()
            {
                Id = item.Id,
                TierId = item.TierId.Value,
                ServiceTypeId = item.ServiceTypeId.Value,
                Name = item.Name,
                Limit = item.Limit.Value,
                Value = item.Value.Value,
                Status = item.Status.Value
            };
        }

        public static RouteViewModel AsRouteViewModel(this Route item)
        {
            return new()
            {
                Id = item.Id,
                PartnerId = item.PartnerId,
                Distance = item.Distance,
                Name = item.Name,
                TotalStation = item.TotalStation,
                Status = item.Status
            };
        }
    }
}