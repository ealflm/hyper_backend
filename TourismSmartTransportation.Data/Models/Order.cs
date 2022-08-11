using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetailOfBookingServices = new HashSet<OrderDetailOfBookingService>();
            OrderDetailOfBusServices = new HashSet<OrderDetailOfBusService>();
            OrderDetailOfPackages = new HashSet<OrderDetailOfPackage>();
            OrderDetailOfRentingServices = new HashSet<OrderDetailOfRentingService>();
            Transactions = new HashSet<Transaction>();
        }

        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? DiscountId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public Guid? PartnerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Discount Discount { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual ICollection<OrderDetailOfBookingService> OrderDetailOfBookingServices { get; set; }
        public virtual ICollection<OrderDetailOfBusService> OrderDetailOfBusServices { get; set; }
        public virtual ICollection<OrderDetailOfPackage> OrderDetailOfPackages { get; set; }
        public virtual ICollection<OrderDetailOfRentingService> OrderDetailOfRentingServices { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
