using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class ShipmentTrackerDto
    {
        public string OrderNumber { get; set; }
     
        public string ShipmentType { get; set; }
        public string Status { get; set; }

        public string Origin { get; set; }
        public string Destination { get; set; }
        public ICollection<TrackerDto> TrackingDetails { get; set; }
    }
}
