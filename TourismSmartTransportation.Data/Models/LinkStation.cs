using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class LinkStation
    {
        public LinkStation()
        {
            LinkRoutes = new HashSet<LinkRoute>();
        }

        public Guid LinkStationId { get; set; }
        public Guid FirstStationId { get; set; }
        public Guid SecondStationId { get; set; }
        public string Content { get; set; }
        public decimal Distance { get; set; }

        public virtual Station FirstStation { get; set; }
        public virtual Station SecondStation { get; set; }
        public virtual ICollection<LinkRoute> LinkRoutes { get; set; }
    }
}
