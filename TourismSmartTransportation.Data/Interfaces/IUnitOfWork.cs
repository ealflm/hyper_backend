using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Admin> AdminRepository { get; }
        IGenericRepository<ActivityDate> ActivityDateRepository { get; }
        IGenericRepository<Card> CardRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Customer> CustomerRepository { get; }
        IGenericRepository<CustomerTierHistory> CustomerTierHistoryRepository { get; }
        IGenericRepository<CustomerTrip> CustomerTripRepository { get; }
        IGenericRepository<Discount> DiscountRepository { get; }
        IGenericRepository<Driver> DriverRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        IGenericRepository<Package> PackageRepository { get; }
        IGenericRepository<PackageStatus> PackageStatusRepository { get; }
        IGenericRepository<Partner> PartnerRepository { get; }
        IGenericRepository<PartnerServiceType> PartnerServiceTypeRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<PublishYear> PublishYearRepository { get; }
        IGenericRepository<RentStation> RentStationRepository { get; }
        IGenericRepository<Route> RouteRepository { get; }
        IGenericRepository<ServiceType> ServiceTypeRepository { get; }
        IGenericRepository<Station> StationRepository { get; }
        IGenericRepository<StationRoute> StationRouteRepository { get; }
        IGenericRepository<Tier> TierRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Trip> TripRepository { get; }
        IGenericRepository<Vehicle> VehicleRepository { get; }
        IGenericRepository<VehicleType> VehicleTypeRepository { get; }
        IGenericRepository<Wallet> WalletRepository { get; }

        tourismsmarttransportationContext Context();

        DatabaseFacade Database();

        Task SaveChangesAsync();
    }
}