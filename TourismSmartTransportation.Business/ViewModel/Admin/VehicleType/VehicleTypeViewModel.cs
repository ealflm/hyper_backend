using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.VehicleType
{
    public class VehicleTypeViewModel
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public int Seats { get; set; }
        public string Fuel { get; set; }
        public int Status { get; set; }
    }
}