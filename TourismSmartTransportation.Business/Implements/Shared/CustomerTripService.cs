using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class CustomerTripService : BaseService, ICustomerTripService
    {
        public CustomerTripService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<List<CustomerTripViewModel>> GetCustomerTripsListForRentingService(CustomerTripSearchModel model)
        {
            var customerTripsList = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.Status == 1)
                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                        customerTrip => customerTrip.VehicleId,
                                        vehicle => vehicle.VehicleId,
                                        (customerTrip, vehicle) => new
                                        {
                                            customerTrip,
                                            vehicle
                                        }
                                    )
                                    .Join(_unitOfWork.ServiceTypeRepository.Query(),
                                        _ => _.vehicle.ServiceTypeId,
                                        serviceType => serviceType.ServiceTypeId,
                                        (_, serviceType) => new
                                        {
                                            CustomerTrip = _.customerTrip,
                                            Vehicle = _.vehicle,
                                            ServiceType = serviceType
                                        }
                                    )
                                    .Where(x => x.ServiceType.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME))
                                    .Select(x => x.CustomerTrip.AsCustomerTripViewModel())
                                    .ToListAsync();

            return customerTripsList;
        }

        public async Task<Response> UpdateStatusCustomerTrip(Guid id, CustomerTripSearchModel model)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(id);
            if (customerTrip == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }

            customerTrip.Status = model.Status;
            _unitOfWork.CustomerTripRepository.Update(customerTrip);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công"
            };
        }
    }
}