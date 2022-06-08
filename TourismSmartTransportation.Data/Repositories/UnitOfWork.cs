using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly tourismsmarttransportationContext _dbContext;

        public IGenericRepository<Admin> AdminRepository { get; }
        public IGenericRepository<ActivityDate> ActivityDateRepository { get; }
        public IGenericRepository<Card> CardRepository { get; }
        public IGenericRepository<Category> CategoryRepository { get; }
        public IGenericRepository<Customer> CustomerRepository { get; }
        public IGenericRepository<CustomerTierHistory> CustomerTierHistoryRepository { get; }
        public IGenericRepository<CustomerTrip> CustomerTripRepository { get; }
        public IGenericRepository<Discount> DiscountRepository { get; }
        public IGenericRepository<Driver> DriverRepository { get; }
        public IGenericRepository<Order> OrderRepository { get; }
        public IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        public IGenericRepository<Package> PackageRepository { get; }
        public IGenericRepository<PackageStatus> PackageStatusRepository { get; }
        public IGenericRepository<Partner> PartnerRepository { get; }
        public IGenericRepository<PartnerServiceType> PartnerServiceTypeRepository { get; }
        public IGenericRepository<Payment> PaymentRepository { get; }
        public IGenericRepository<PublishYear> PublishYearRepository { get; }
        public IGenericRepository<RentStation> RentStationRepository { get; }
        public IGenericRepository<Route> RouteRepository { get; }
        public IGenericRepository<ServiceType> ServiceTypeRepository { get; }
        public IGenericRepository<Station> StationRepository { get; }
        public IGenericRepository<StationRoute> StationRouteRepository { get; }
        public IGenericRepository<Tier> TierRepository { get; }
        public IGenericRepository<Transaction> TransactionRepository { get; }
        public IGenericRepository<Trip> TripRepository { get; }
        public IGenericRepository<Vehicle> VehicleRepository { get; }
        public IGenericRepository<VehicleType> VehicleTypeRepository { get; }
        public IGenericRepository<Wallet> WalletRepository { get; }
        public IGenericRepository<PriceListOfBusService> PriceListOfBusServiceRepository { get; }
        public IGenericRepository<PriceListOfBookingService> PriceListOfBookingServiceRepository { get; }


        public UnitOfWork(
            tourismsmarttransportationContext dbContext,
            IGenericRepository<Admin> adminRepository,
            IGenericRepository<ActivityDate> activityDateRepository,
            IGenericRepository<Card> cardRepository,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<CustomerTierHistory> customerTierHistoryRepository,
            IGenericRepository<CustomerTrip> customerTripRepository,
            IGenericRepository<Discount> discountRepository,
            IGenericRepository<Driver> driverRepository,
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetail> orderDetailRepository,
            IGenericRepository<Package> packageRepository,
            IGenericRepository<PackageStatus> packageStatusRepository,
            IGenericRepository<Partner> partnerRepository,
            IGenericRepository<PartnerServiceType> partnerServiceTypeRepository,
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<PublishYear> publishYearRepository,
            IGenericRepository<RentStation> rentStationRepository,
            IGenericRepository<Route> routeRepository,
            IGenericRepository<ServiceType> serviceTypeRepository,
            IGenericRepository<Station> stationRepository,
            IGenericRepository<StationRoute> stationRouteRepository,
            IGenericRepository<Tier> tierRepository,
            IGenericRepository<Transaction> transactionRepository,
            IGenericRepository<Trip> tripRepository,
            IGenericRepository<Vehicle> vehicleRepository,
            IGenericRepository<VehicleType> vehicleTypeRepository,
            IGenericRepository<Wallet> walletRepository,
            IGenericRepository<PriceListOfBusService> priceListOfBusService,
            IGenericRepository<PriceListOfBookingService> priceListOfBookingService
            )
        {
            _dbContext = dbContext;

            AdminRepository = adminRepository;
            ActivityDateRepository = activityDateRepository;
            CardRepository = cardRepository;
            CategoryRepository = categoryRepository;
            CustomerRepository = customerRepository;
            CustomerTierHistoryRepository = customerTierHistoryRepository;
            CustomerTripRepository = customerTripRepository;
            DiscountRepository = discountRepository;
            DriverRepository = driverRepository;
            OrderRepository = orderRepository;
            OrderDetailRepository = orderDetailRepository;
            PackageRepository = packageRepository;
            PackageStatusRepository = packageStatusRepository;
            PartnerRepository = partnerRepository;
            PartnerServiceTypeRepository = partnerServiceTypeRepository;
            PaymentRepository = paymentRepository;
            PublishYearRepository = publishYearRepository;
            RentStationRepository = rentStationRepository;
            RouteRepository = routeRepository;
            ServiceTypeRepository = serviceTypeRepository;
            StationRepository = stationRepository;
            StationRouteRepository = stationRouteRepository;
            TransactionRepository = transactionRepository;
            TierRepository = tierRepository;
            TransactionRepository = transactionRepository;
            TripRepository = tripRepository;
            VehicleRepository = vehicleRepository;
            VehicleTypeRepository = vehicleTypeRepository;
            WalletRepository = walletRepository;
            PriceListOfBusServiceRepository = priceListOfBusService;
            PriceListOfBookingServiceRepository = priceListOfBookingService;
        }

        public tourismsmarttransportationContext Context()
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