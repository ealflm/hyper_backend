using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Trip
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid RouteId { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public int DayOfWeek { get; set; }
        public Guid CompanyId { get; set; }
        public int Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual Route Route { get; set; }
    }
}
