using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Hubs
{
    public interface INotificationHub
    {
        Task FindDriver(string json, string isClient = "true");
        string GetDriversListMatching(string json);
        Task<bool> CheckAcceptedRequest(string json, string status);
        Task<bool> CancelBooking(string customerId);
        Task DriverArrived(string driverId);
        Task DriverPickedUp(string driverId);
        Task CompletedBooking(string driverId);
        Task CanceledFinding(string customerId);
        Task<bool> OpenDriver(string json);
        Task<bool> CloseDriver(string driverId);
        Task<string> GetDriverByCustomer(string customerId);
        Task<string> GetDriverStatus(string driverId);
        Task<string> GetCustomerStatus(string customerId);
        Task LoadVehiclesList();
        Task AutoCanceledBookingRequestAfterSpecificTime();
    }
}