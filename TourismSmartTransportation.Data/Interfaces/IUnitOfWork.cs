using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Admin> AdminRepository { get; }
        IGenericRepository<Card> CardRepository { get; }
        IGenericRepository<Company> CompanyRepository { get; }
        IGenericRepository<Customer> CustomerRepository { get; }
        IGenericRepository<Discount> DiscountRepository { get; }
        IGenericRepository<Driver> DriverRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<RentStation> RentStationRepository { get; }
        IGenericRepository<Route> RouteRepository { get; }
        IGenericRepository<RouteStation> RouteStationRepository { get; }
        IGenericRepository<Service> ServiceRepository { get; }
        IGenericRepository<ServiceDetail> ServiceDetailRepository { get; }
        IGenericRepository<ServiceType> ServiceTypeRepository { get; }
        IGenericRepository<Station> StationRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Trip> TripRepository { get; }
        IGenericRepository<Vehicle> VehicleRepository { get; }
        IGenericRepository<VehicleType> VehicleTypeRepository { get; }
        IGenericRepository<Wallet> WalletRepository { get; }

        TourismSmartTransportationContext Context();

        DatabaseFacade Database();

        Task SaveChangesAsync();
    }
}