using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderDestinationAgent
{
    public class GetDestinationAgentPartialDeliveryResponse
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public DateTime? PartialDeliveryDate { get; set; }
        public int? WeightDeliveredLb { get; set; }
    }
}