﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.ServiceOrderRoadFreight
{
    public class CreateRoadFreightLegRequest
    {
        public string OriginLoCode { get; set; }
        public string OriginName { get; set; }
        public DateTime? EstimatedDepartureDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public string DestinationLoCode { get; set; }
        public string DestinationName { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public string ProNumber { get; set; }
    }
}
