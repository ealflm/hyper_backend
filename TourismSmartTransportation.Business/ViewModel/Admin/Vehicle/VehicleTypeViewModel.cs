using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Vehicle
{
    public class VehicleTypeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
        public int Fuel { get; set; }
        public int Status { get; set; }
    }
}