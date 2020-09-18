using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderThirdParty
{
    public class GetThirdPartyServiceResponse
    {
        public int Id { get; set; }
        public string OriginOrDestination { get; set; }
        public string ThirdPartyServiceName { get; set; }
        public decimal? Cost { get; set; }
        public string CostType { get; set; }
        public int? Quantity { get; set; }
    }
}