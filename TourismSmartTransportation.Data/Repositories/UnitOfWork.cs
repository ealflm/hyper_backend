using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TourismSmartTransportationContext _dbContext;

        public IGenericRepository<Admin> AdminRepository { get; }
        public IGenericRepository<Card> CardRepository { get; }
        public IGenericRepository<Company> CompanyRepository { get; }
        public IGenericRepository<CompanyTrip> CompanyTripRepository { get; }
        public IGenericRepository<Customer> CustomerRepository { get; }
        public IGenericRepository<Discount> DiscountRepository { get; }
        public IGenericRepository<Driver> DriverRepository { get; }
        public IGenericRepository<Payment> PaymentRepository { get; }
        public IGenericRepository<Rent> RentRepository { get; }
        public IGenericRepository<RentStation> RentStationRepository { get; }
        public IGenericRepository<Route> RouteRepository { get; }
        public IGenericRepository<RouteStation> RouteStationRepository { get; }
        public IGenericRepository<Service> ServiceRepository { get; }
        public IGenericRepository<ServiceDetail> ServiceDetailRepository { get; }
        public IGenericRepository<ServiceDiscount> ServiceDiscountRepository { get; }
        public IGenericRepository<ServiceType> ServiceTypeRepository { get; }
        public IGenericRepository<Station> StationRepository { get; }
        public IGenericRepository<Transaction> TransactionRepository { get; }
        public IGenericRepository<Trip> TripRepository { get; }
        public IGenericRepository<Vehicle> VehicleRepository { get; }
        public IGenericRepository<VehicleType> VehicleTypeRepository { get; }
        public IGenericRepository<Wallet> WalletRepository { get; }
        public 
            IGenericRepository<WalletService> WalletServiceRepository { get; }

        public UnitOfWork(TourismSmartTransportationContext dbContext,
                        IGenericRepository<Admin> adminRepository,
                        IGenericRepository<Card> cardRepository,
                        IGenericRepository<Company> companyRepository,
                        IGenericRepository<CompanyTrip> companyTripRepository,
                        IGenericRepository<Customer> customerRepository,
                        IGenericRepository<Discount> discountRepository,
                        IGenericRepository<Driver> driverRepository,
                        IGenericRepository<Payment> paymentRepository,
                        IGenericRepository<Rent> rentRepository,
                        IGenericRepository<RentStation> rentStationRepository,
                        IGenericRepository<Route> routeRepository,
                        IGenericRepository<RouteStation> routeStationRepository,
                        IGenericRepository<Service> serviceRepository,
                        IGenericRepository<ServiceDetail> serviceDetailRepository,
                        IGenericRepository<ServiceDiscount> serviceDiscountRepository,
                        IGenericRepository<ServiceType> serviceTypeRepository,
                        IGenericRepository<Station> stationRepository,
                        IGenericRepository<Transaction> transactionRepository,
                        IGenericRepository<Trip> tripRepository,
                        IGenericRepository<Vehicle> vehicleRepository,
                        IGenericRepository<VehicleType> vehicleTypeRepository,
                        IGenericRepository<Wallet> walletRepository,
                        IGenericRepository<WalletService> walletServiceRepository
            )
        {
            _dbContext = dbContext;

            AdminRepository = adminRepository;
            CardRepository = cardRepository;
            CompanyRepository = companyRepository;
            CompanyTripRepository = companyTripRepository;
            CustomerRepository = customerRepository;
            DiscountRepository = discountRepository;
            DriverRepository = driverRepository;
            PaymentRepository = paymentRepository;
            RentRepository = rentRepository;
            RentStationRepository = rentStationRepository;
            RouteRepository = routeRepository;
            RouteStationRepository = routeStationRepository;
            ServiceRepository = serviceRepository;
            ServiceDetailRepository = serviceDetailRepository;
            ServiceDiscountRepository = serviceDiscountRepository;
            ServiceTypeRepository = serviceTypeRepository;
            StationRepository = stationRepository;
            TransactionRepository = transactionRepository;
            TripRepository = tripRepository;
            VehicleRepository = vehicleRepository;
            VehicleTypeRepository = vehicleTypeRepository;
            WalletRepository = walletRepository;
            WalletServiceRepository = walletServiceRepository;
        }

        public TourismSmartTransportationContext Context()
        {
            return _dbContext;
        }

        public DatabaseFacade Database()
        {
            return _dbContext.Database;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}