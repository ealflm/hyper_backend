using System;

namespace TourismSmartTransportation.Business.Hubs.Models
{
    public class InterValLoopModel
    {
        public string CustomerId { get; set; }
        public DateTime CustomerTimeOut { get; set; }
        public string DriverId { get; set; }
        public DateTime DriverTimeOut { get; set; }
    }
}