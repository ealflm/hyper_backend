using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.SearchModel.Admin.ServiceType;
using TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBusServiceViewModel;
using TourismSmartTransportation.Business.ViewModel.Admin.CategoryManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBookingServiceViewModel;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceRentingServiceViewModel;
using TourismSmartTransportation.Business.ViewModel.Admin.CardManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Extensions
{
    public static class EntitiesExtension
    {


        public static DiscountViewModel AsDiscountViewModel(this Discount item)
        {
            return new()
            {
                Id = item.DiscountId,
                ServiceTypeId = item.ServiceTypeId,
                Title = item.Title,
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
                Id = item.VehicleTypeId,
                Label = item.Label,
                Fuel = item.Fuel,
                Seats = item.Seats,
                Status = item.Status
            };
        }

        public static PartnerViewModel AsPartnerViewModel(this Partner item)
        {
            return new()
            {
                Id = item.PartnerId,
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
                Id = item.StationId,
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
                Id = item.CustomerId,
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
                Id = item.RentStationId,
                PartnerId = item.PartnerId,
                Title = item.Title,
                Description = item.Description,
                Address = item.Address,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                Status = item.Status
            };
        }

        public static OrderViewModel AsOrderViewModel(this Order item)
        {
            return new()
            {
                Id = item.OrderId,
                CustomerId = item.CustomerId,
                ServiceTypeId = item.ServiceTypeId != null ? item.ServiceTypeId.Value : null,
                DiscountId = item.DiscountId != null ? item.DiscountId.Value : null,
                CreatedDate = item.CreatedDate,
                TotalPrice = item.TotalPrice,
                Status = item.Status
            };
        }
        public static OrderDetailOfPackageViewModel AsOrderDetailOfPackageViewModel(this OrderDetailOfPackage item)
        {
            return new()
            {
                OrderId = item.OrderId,
                PackageId = item.PackageId,
                Content = item.Content,
                Price = item.Price,
                Quantity = item.Quantity,
                Status = item.Status
            };
        }

        public static OrderDetailOfBookingServiceViewModel AsOrderDetailOfBookingServiceViewModel(this OrderDetailOfBookingService item)
        {
            return new()
            {
                OrderId = item.OrderId,
                PriceOfBookingServiceId = item.PriceOfBookingServiceId,
                Content = item.Content,
                Price = item.Price,
                Quantity = item.Quantity,
                Status = item.Status
            };
        }

        public static OrderDetailOfBusServiceViewModel AsOrderDetailOfBusServiceViewModel(this OrderDetailOfBusService item)
        {
            return new()
            {
                OrderId = item.OrderId,
                PriceOfBusServiceId = item.PriceOfBusServiceId,
                Content = item.Content,
                Price = item.Price,
                Quantity = item.Quantity,
                Status = item.Status
            };
        }

        public static OrderDetailOfRentingServiceViewModel AsOrderDetailOfRentingServiceViewModel(this OrderDetailOfRentingService item)
        {
            return new()
            {
                OrderId = item.OrderId,
                PriceOfRentingServiceId = item.PriceOfRentingService,
                Content = item.Content,
                Price = item.Price,
                Quantity = item.Quantity,
                Status = item.Status
            };
        }

        public static TransactionViewModel AsTransactionViewModel(this Transaction item)
        {
            return new()
            {
                OrderId = item.OrderId,
                WalletId = item.WalletId,
                Content = item.Content,
                Amount = item.Amount,
                CreatedDate = item.CreatedDate,
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
                Id = item.ServiceTypeId,
                Content = item.Content,
                Name = item.Name,
                Status = item.Status
            };
        }

        public static PackageViewModel AsPackageViewModel(this Package item)
        {
            return new()
            {
                Id = item.PackageId,
                Name = item.Name,
                PeopleQuanitty = item.PeopleQuanitty,
                Duration = item.Duration,
                Description = item.Description,
                PhotoUrl = item.PhotoUrl,
                PromotedTitle = item.PromotedTitle,
                Price = item.Price,
                Status = item.Status
            };
        }

        public static PackageItemViewModel AsPackageItemViewModel(this PackageItem item)
        {
            return new()
            {
                PackageId = item.PackageId,
                ServiceTypeId = item.ServiceTypeId,
                Name = item.Name,
                Limit = item.Limit,
                Value = item.Value,
                Status = item.Status
            };
        }

        public static PackageItem AsPackageItemData(this CreatePackageItemModel item)
        {
            return new()
            {
                PackageId = item.PackageId,
                ServiceTypeId = item.ServiceTypeId,
                Name = item.Name,
                Limit = item.Limit,
                Value = item.Value,
                Status = item.Status.Value
            };
        }

        public static PackageItem AsPackageItemData(this UpdatePackageItemModel item)
        {
            return new()
            {
                PackageId = item.PackageId.Value,
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
                Id = item.RouteId,
                PartnerId = item.PartnerId,
                Distance = item.Distance,
                Name = item.Name,
                TotalStation = item.TotalStation,
                Status = item.Status
            };
        }

        public static StationRoute AsStationRouteData(this CreateStationRoute item)
        {
            return new()
            {
                OrderNumber = item.OrderNumber,
                StationId = item.StationId,
                RouteId = item.RouteId,
                Distance = item.Distance,
                Status = 1
            };
        }

        public static PriceOfBusServiceViewModel AsPriceOfBusServiceViewModel(this PriceOfBusService item)
        {
            return new()
            {
                Id = item.PriceOfBusServiceId,
                BasePriceId = item.BasePriceId,
                MinDistance = item.MinDistance,
                MaxDistance = item.MaxDistance,
                MinStation = item.MinStation,
                MaxStation = item.MaxStation,
                Price = item.Price,
                Mode = item.Mode,
                Status = item.Status
            };
        }

        public static PriceOfBookingServiceViewModel AsPriceOfBookingServiceViewModel(this PriceOfBookingService item)
        {
            return new()
            {
                Id = item.PriceOfBookingServiceId,
                FixedDistance = item.FixedDistance,
                FixedPrice = item.FixedPrice,
                PricePerKilometer = item.PricePerKilometer,
                VehicleTypeId = item.VehicleTypeId,
                Status = item.Status
            };
        }

        public static PriceOfRentingServiceViewModel AsPriceOfRentingService(this PriceOfRentingService item)
        {
            return new()
            {
                Id = item.PriceOfRentingServiceId,
                CategoryId = item.CategoryId,
                PublishYearId = item.PublishYearId,
                FixedPrice = item.FixedPrice,
                HolidayPrice = item.HolidayPrice,
                MaxTime = item.MaxTime,
                MinTime = item.MinTime,
                PricePerHour = item.PricePerHour,
                WeekendPrice = item.WeekendPrice,
                Status = item.Status
            };
        }

        public static CategoryViewModel AsCategoryViewModel(this Category item)
        {
            return new()
            {
                Id = item.CategoryId,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status
            };
        }

        public static PublishYearViewModel AsPublishYearViewModel(this PublishYear item)
        {
            return new()
            {
                Id = item.PublishYearId,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status
            };
        }

        public static CardViewModel AsCardViewModel(this Card item)
        {
            return new()
            {
                Id = item.CardId,
                CustomerId = item.CustomerId,
                Uid = item.Uid,
                Status = item.Status
            };
        }

        public static VehicleViewModel AsVehicleViewModel(this Vehicle item)
        {
            return new()
            {
                Id = item.VehicleId,
                Color = item.Color,
                LicensePlates = item.LicensePlates,
                Name = item.Name,
                PartnerId = item.PartnerId,
                PriceRentingId = item.PriceRentingId,
                RentStationId = item.RentStationId,
                ServiceTypeId = item.ServiceTypeId,
                VehicleTypeId = item.VehicleTypeId,
                IsRunning = item.IsRunning == null ? item.IsRunning : item.IsRunning.Value,
                Status = item.Status
            };
        }

        public static DriverViewModel AsDriverViewModel(this Driver item)
        {
            return new()
            {
                Id = item.DriverId,
                PartnerId = item.PartnerId,
                VehicleId = item.VehicleId != null ? item.VehicleId.Value : null,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Phone = item.Phone,
                DateOfBirth = item.DateOfBirth != null ? item.DateOfBirth.Value : null,
                Gender = item.Gender,
                PhotoUrl = item.PhotoUrl,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                Status = item.Status
            };
        }

        public static TripViewModel AsTripViewModel(this Trip item)
        {
            return new()
            {
                TripId = item.TripId,
                RouteId = item.RouteId,
                DriverId = item.DriverId,
                VehicleId = item.VehicleId,
                TripName = item.TripName,
                DayOfWeek = item.DayOfWeek,
                TimeStart = item.TimeStart,
                TimeEnd = item.TimeEnd,
                Status = item.Status
            };
        }

        public static BasePriceOfBusServiceViewModel AsBasePriceOfBusServiceViewModel(this BasePriceOfBusService item)
        {
            return new()
            {
                Id = item.BasePriceOfBusServiceId,
                MinDistance = item.MinDistance,
                MaxDistance = item.MaxDistance,
                Price = item.Price,
                Status = item.Status
            };
        }

        public static WalletViewModel AsWalletViewModel(this Wallet item)
        {
            return new()
            {
                WalletId = item.WalletId,
                CustomerId = item.CustomerId.Value,
                AccountBalance = item.AccountBalance,
                Status = item.Status
            };
        }

        public static CustomerTripViewModel AsCustomerTripViewModel(this CustomerTrip item)
        {
            return new()
            {
                CustomerTripId = item.CustomerTripId,
                CustomerId = item.CustomerId,
                RouteId = item.RouteId != null ? item.RouteId.Value : null,
                VehicleId = item.VehicleId,
                Distance = item.Distance != null ? item.Distance.Value : null,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                RentDeadline = item.RentDeadline,
                Coordinates = item.Coordinates,
                Status = item.Status,
                // LongitudeOfPickUpPoint = item.LongitudeOfPickUpPoint != null ? item.LongitudeOfPickUpPoint.Value : null,
                // LatitudeOfPickUpPoint = item.LatitudeOfPickUpPoint != null ? item.LatitudeOfPickUpPoint.Value : null,
                // LongitudeOfDestination = item.LongitudeOfDestination != null ? item.LongitudeOfDestination.Value : null,
                // LatitudeOfDestination = item.LatitudeOfDestination != null ? item.LatitudeOfDestination.Value : null,
            };
        }
    }
}