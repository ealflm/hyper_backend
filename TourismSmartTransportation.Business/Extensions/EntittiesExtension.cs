using TourismSmartTransportation.Business.ViewModel.Admin.CompanyManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.Vehicle;
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
                Time = item.Time,
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
                Name = item.Name,
                Fuel = item.Fuel,
                Seats = item.Seats,
                Status = item.Status
            };
        }

        public static CompanyViewModel AsCompanyViewModel(this Company item)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                UserName = item.UserName,
                PhotoUrl = item.PhotoUrl,
                Status = item.Status
            };
        }

        public static StationViewModel AsStationViewModel(this Station item)
        {
            return new()
            {
                Id = item.Id,
                Address = item.Address,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Name = item.Name,
                Status = item.Status
            };
        }
    }
}