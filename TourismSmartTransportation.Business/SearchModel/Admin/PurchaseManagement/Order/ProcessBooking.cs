namespace TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order
{
    public class ProcessBookingModel
    {
        public string CustomerId { get; set; }
        public bool ChangeCustomerTrip { get; set; }
        public bool ChangeOrder { get; set; }
        public int CustomerTripStatus { get; set; }
        public int OrderStatus { get; set; }
    }
}