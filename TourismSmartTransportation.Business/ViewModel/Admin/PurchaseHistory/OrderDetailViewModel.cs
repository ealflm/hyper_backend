using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory
{
    public class OrderDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid TierId { get; set; }
        public Guid PriceDefaultId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    }
}
