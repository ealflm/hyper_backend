using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class HyperHubService : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public HyperHubService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}