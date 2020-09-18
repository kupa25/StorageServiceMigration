using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight
{
    public class GetAirFreightLegResponse
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public string OriginIataCode { get; set; }
        public string OriginName { get; set; }
        public DateTime? EstimatedDepartureDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public string DestinationIataCode { get; set; }
        public string DestinationName { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public string AirlinePrefix { get; set; }
        public string FlightNumber { get; set; }
    }
}