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
        public IGenericRepository<BasePriceOfBusService> BasePriceOfBusServiceRepository { get; }
        public IGenericRepository<Card> CardRepository { get; }
        public IGenericRepository<Category> CategoryRepository { get; }
        public IGenericRepository<Customer> CustomerRepository { get; }
        public IGenericRepository<CustomerTrip> CustomerTripRepository { get; }
        public IGenericRepository<Discount> DiscountRepository { get; }
        public IGenericRepository<Driver> DriverRepository { get; }
        public IGenericRepository<FeedbackForDriver> FeedbackForDriverRepository { get; }
        public IGenericRepository<FeedbackForVehicle> FeedbackForVehicleRepository { get; }
        public IGenericRepository<Order> OrderRepository { get; }
        public IGenericRepository<OrderDetailOfBookingService> OrderDetailOfBookingServiceRepository { get; }
        public IGenericRepository<OrderDetailOfBusService> OrderDetailOfBusServiceRepository { get; }
        public IGenericRepository<OrderDetailOfPackage> OrderDetailOfPackageRepository { get; }
        public IGenericRepository<OrderDetailOfRentingService> OrderDetailOfRentingServiceRepository { get; }
        public IGenericRepository<Package> PackageRepository { get; }
        public IGenericRepository<PackageItem> PackageItemRepository { get; }
        public IGenericRepository<Partner> PartnerRepository { get; }
        public IGenericRepository<PartnerServiceType> PartnerServiceTypeRepository { get; }
        public IGenericRepository<PriceOfBookingService> PriceOfBookingServiceRepository { get; }
        public IGenericRepository<PriceOfBusService> PriceOfBusServiceRepository { get; }
        public IGenericRepository<PriceOfRentingService> PriceOfRentingServiceRepository { get; }
        public IGenericRepository<PublishYear> PublishYearRepository { get; }
        public IGenericRepository<RentStation> RentStationRepository { get; }
        public IGenericRepository<Route> RouteRepository { get; }
        public IGenericRepository<RoutePriceBusing> RoutePriceBusingRepository { get; }
        public IGenericRepository<ServiceType> ServiceTypeRepository { get; }
        public IGenericRepository<Station> StationRepository { get; }
        public IGenericRepository<StationRoute> StationRouteRepository { get; }
        public IGenericRepository<Transaction> TransactionRepository { get; }
        public IGenericRepository<Trip> TripRepository { get; }
        public IGenericRepository<Vehicle> VehicleRepository { get; }
        public IGenericRepository<VehicleType> VehicleTypeRepository { get; }
        public IGenericRepository<Wallet> WalletRepository { get; }

        public UnitOfWork(
            tourismsmarttransportationContext dbContext,
            IGenericRepository<Admin> adminRepository,
            IGenericRepository<BasePriceOfBusService> basePriceOfBusServiceRepository,
            IGenericRepository<Card> cardRepository,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<CustomerTrip> customerTripRepository,
            IGenericRepository<Discount> discountRepository,
            IGenericRepository<Driver> driverRepository,
            IGenericRepository<FeedbackForDriver> feedbackForDriverRepository,
            IGenericRepository<FeedbackForVehicle> feedbackForVehicleRepository,
            IGenericRepository<Order> orderRepository,
            IGenericRepository<OrderDetailOfBookingService> orderDetailOfBookingServiceRepository,
            IGenericRepository<OrderDetailOfBusService> orderDetailOfBusServiceRepository,
            IGenericRepository<OrderDetailOfPackage> orderDetailOfPackageRepository,
            IGenericRepository<OrderDetailOfRentingService> orderDetailOfRentingServiceRepository,
            IGenericRepository<Package> packageRepository,
            IGenericRepository<PackageItem> packageItemRepository,
            IGenericRepository<Partner> partnerRepository,
            IGenericRepository<PartnerServiceType> partnerServiceTypeRepository,
            IGenericRepository<PriceOfBookingService> priceOfBookingServiceRepository,
            IGenericRepository<PriceOfBusService> priceOfBusServiceRepository,
            IGenericRepository<PriceOfRentingService> priceOfRentingServiceRepository,
            IGenericRepository<PublishYear> publishYearRepository,
            IGenericRepository<RentStation> rentStationRepository,
            IGenericRepository<Route> routeRepository,
            IGenericRepository<RoutePriceBusing> routePriceBusingRepository,
            IGenericRepository<ServiceType> serviceTypeRepository,
            IGenericRepository<Station> stationRepository,
            IGenericRepository<StationRoute> stationRouteRepository,
            IGenericRepository<Transaction> transactionRepository,
            IGenericRepository<Trip> tripRepository,
            IGenericRepository<Vehicle> vehicleRepository,
            IGenericRepository<VehicleType> vehicleTypeRepository,
            IGenericRepository<Wallet> walletRepository
            )
        {
            _dbContext = dbContext;

            AdminRepository = adminRepository;
            BasePriceOfBusServiceRepository = basePriceOfBusServiceRepository;
            CardRepository = cardRepository;
            CategoryRepository = categoryRepository;
            CustomerRepository = customerRepository;
            CustomerTripRepository = customerTripRepository;
            DiscountRepository = discountRepository;
            DriverRepository = driverRepository;
            FeedbackForDriverRepository = feedbackForDriverRepository;
            FeedbackForVehicleRepository = feedbackForVehicleRepository;
            OrderRepository = orderRepository;
            OrderDetailOfBookingServiceRepository = orderDetailOfBookingServiceRepository;
            OrderDetailOfBusServiceRepository = orderDetailOfBusServiceRepository;
            OrderDetailOfPackageRepository = orderDetailOfPackageRepository;
            OrderDetailOfRentingServiceRepository = orderDetailOfRentingServiceRepository;
            PackageRepository = packageRepository;
            PackageItemRepository = packageItemRepository;
            PartnerRepository = partnerRepository;
            PartnerServiceTypeRepository = partnerServiceTypeRepository;
            PriceOfBookingServiceRepository = priceOfBookingServiceRepository;
            PriceOfBusServiceRepository = priceOfBusServiceRepository;
            PriceOfRentingServiceRepository = priceOfRentingServiceRepository;
            PublishYearRepository = publishYearRepository;
            RentStationRepository = rentStationRepository;
            RouteRepository = routeRepository;
            RoutePriceBusingRepository = routePriceBusingRepository;
            ServiceTypeRepository = serviceTypeRepository;
            StationRepository = stationRepository;
            StationRouteRepository = stationRouteRepository;
            TransactionRepository = transactionRepository;
            TripRepository = tripRepository;
            VehicleRepository = vehicleRepository;
            VehicleTypeRepository = vehicleTypeRepository;
            WalletRepository = walletRepository;
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