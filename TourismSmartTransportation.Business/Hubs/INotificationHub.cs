using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Hubs.Models;

namespace TourismSmartTransportation.Business.Hubs
{
    public interface INotificationHub
    {
        Task FindDriver(string json, string isClient = "true", string isBackgroudService = "false");
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
        Task<Dictionary<string, Tuple<TransferHubModel, Dictionary<string, HashSet<string>>>>> AutoCanceledBookingRequestAfterSpecificTime();
    }
}