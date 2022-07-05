
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer.Authorization
{
    public class OTPResponse
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string RequestId { get; set; }
    }
}
