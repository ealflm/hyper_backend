using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Admin> AdminRepository { get; }
        IGenericRepository<BasePriceOfBusService> BasePriceOfBusServiceRepository { get; }
        IGenericRepository<Card> CardRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Customer> CustomerRepository { get; }
        IGenericRepository<CustomerTrip> CustomerTripRepository { get; }
        IGenericRepository<Discount> DiscountRepository { get; }
        IGenericRepository<Driver> DriverRepository { get; }
        IGenericRepository<FeedbackForDriver> FeedbackForDriverRepository { get; }
        IGenericRepository<FeedbackForVehicle> FeedbackForVehicleRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderDetailOfBookingService> OrderDetailOfBookingServiceRepository { get; }
        IGenericRepository<OrderDetailOfBusService> OrderDetailOfBusServiceRepository { get; }
        IGenericRepository<OrderDetailOfPackage> OrderDetailOfPackageRepository { get; }
        IGenericRepository<OrderDetailOfRentingService> OrderDetailOfRentingServiceRepository { get; }
        IGenericRepository<Package> PackageRepository { get; }
        IGenericRepository<PackageItem> PackageItemRepository { get; }
        IGenericRepository<Partner> PartnerRepository { get; }
        IGenericRepository<PartnerServiceType> PartnerServiceTypeRepository { get; }
        IGenericRepository<PriceOfBookingService> PriceOfBookingServiceRepository { get; }
        IGenericRepository<PriceOfBusService> PriceOfBusServiceRepository { get; }
        IGenericRepository<PriceOfRentingService> PriceOfRentingServiceRepository { get; }
        IGenericRepository<PublishYear> PublishYearRepository { get; }
        IGenericRepository<RentStation> RentStationRepository { get; }
        IGenericRepository<Route> RouteRepository { get; }
        IGenericRepository<RoutePriceBusing> RoutePriceBusingRepository { get; }
        IGenericRepository<ServiceType> ServiceTypeRepository { get; }
        IGenericRepository<Station> StationRepository { get; }
        IGenericRepository<StationRoute> StationRouteRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Trip> TripRepository { get; }
        IGenericRepository<Vehicle> VehicleRepository { get; }
        IGenericRepository<VehicleType> VehicleTypeRepository { get; }
        IGenericRepository<Wallet> WalletRepository { get; }
        IGenericRepository<LinkStation> LinkStationRepository { get; }
        IGenericRepository<LinkRoute> LinkRouteRepository { get; }
        tourismsmarttransportationContext Context();

        DatabaseFacade Database();

        Task SaveChangesAsync();
    }
}