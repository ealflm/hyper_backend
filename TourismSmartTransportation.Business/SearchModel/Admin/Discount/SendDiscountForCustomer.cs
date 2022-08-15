using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class SendDiscountToCustomer
    {
        [Required]
        public Guid DiscountId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
    }
}