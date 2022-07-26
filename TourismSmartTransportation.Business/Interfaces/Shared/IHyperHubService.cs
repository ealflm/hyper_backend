using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface IHyperHubService
    {
        Task BroadcastMessage(string message);
    }
}