using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.SearchModel.Admin.ServiceType;
using TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement;
using TourismSmartTransportation.Business.ViewModel.Shared;

namespace TourismSmartTransportation.Business.Extensions
{
    public static class EntitiesExtension
    {
        public static ServiceViewModel AsServiceViewModel(this Service item)
        {
            return new()
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                TimeStart = item.TimeStart,
                TimeEnd = item.TimeEnd,
                PhotoUrls = item.PhotoUrls,
                Status = item.Status
            };
        }

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
                PhotoUrls = item.PhotoUrls,
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
                CreatedTime = item.CreatedTime,
                OrderId = item.OrderId,
                Price = item.Price,
                PriceDefaultId = item.PriceDefaultId.GetValueOrDefault(),
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
    }
}