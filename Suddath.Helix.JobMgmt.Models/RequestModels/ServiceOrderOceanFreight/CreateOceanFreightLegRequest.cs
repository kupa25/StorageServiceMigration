using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateOceanFreightLegRequest
    {
        public string OriginLoCode { get; set; }
        public string OriginName { get; set; }
        public DateTime? EstimatedDepartureDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public string DestinationLoCode { get; set; }
        public string DestinationName { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public string VesselName { get; set; }
        public string VoyageNumber { get; set; }
    }
}
