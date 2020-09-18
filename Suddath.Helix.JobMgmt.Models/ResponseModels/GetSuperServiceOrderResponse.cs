using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetSuperServiceOrderResponse
    {
        public int Id { get; set; }
        public string DisplayId { get; set; }
        public int SuperServiceId { get; set; }
        public string SuperServiceName { get; set; }
        public string SuperServiceModeName { get; set; }
        public string SuperServiceIconName { get; set; }
        public string Status { get; set; }
        public IEnumerable<GetServiceOrderResponse> ServiceOrders { get; set; }
    }
}