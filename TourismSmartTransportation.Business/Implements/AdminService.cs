using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.ViewModel;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements
{
    public class AdminService : BaseService, IAdminService
    {
        public AdminService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<List<AdminViewModel>> GetAdmin()
        {
            var admins = await _unitOfWork.AdminRepository.Query()
                .Select(x => new AdminViewModel()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    PhotoUrl = x.PhotoUrl,
                    Status = x.Status
                }).ToListAsync();
            return admins;
        }
    }
}
