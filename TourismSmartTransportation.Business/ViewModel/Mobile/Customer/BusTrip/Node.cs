using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer.BusTrip
{
    class Node
    {
        public Node(Guid value, Node parent, Guid stationId)
        {
            Value = value;
            Parent = parent;
            StationId = stationId;
            if (parent != null)
            {
                Count = parent.Count + 1;
            }
            else
            {
                Count = 1;
            }
            
        }

        public Guid Value { get; set; }
        public Node Parent { get; set; }
        public int Count { get; set; }
        public Guid StationId { get; set; }
    }
}
