using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class SendDiscountToCustomer
    {
        [Required]
        public Guid DiscountId { get; set; }

        [Required]
        public List<Guid> CustomerIdList { get; set; }
    }
}