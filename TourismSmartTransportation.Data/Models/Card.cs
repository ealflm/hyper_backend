using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Card
    {
        public Guid CardId { get; set; }
        public Guid? CustomerId { get; set; }
        public string Uid { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
