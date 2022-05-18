using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement;
using TourismSmartTransportation.Data.Models;

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
                TimeStart = item.TimeStart,
                TimeEnd = item.TimeEnd,
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
                PhotoUrl = item.PhotoUrl,
                Gender = item.Gender,
                Phone = item.Phone,
                Address1 = item.Address1,
                Address2 = item.Address2,
                DateOfBirth = item.DateOfBirth,
                CreatedDate = item.CreatedDate.Value,
                ModifiedDate = item.ModifiedDate.Value,
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
                FirstName = item.FirstName,
                LastName = item.LastName,
                Gender = item.Gender,
                DateOfBirth = item.DateOfBirth,
                Email = item.Email,
                Phone = item.Phone,
                PhotoUrl = item.PhotoUrl,
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
    }
}