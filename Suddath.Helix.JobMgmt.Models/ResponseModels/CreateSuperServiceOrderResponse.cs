using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class CreateSuperServiceOrderResponse
    {
        public int Id { get; set; }
        public string DisplayId { get; set; }
        public int SuperServiceId { get; set; }
        public string SuperServiceName { get; set; }
        public int SuperServiceModeId { get; set; }
        public string SuperServiceModeName { get; set; }
        public string SuperServiceIconName { get; set; }
        public IEnumerable<CreateServiceOrderResponse> ServiceOrders { get; set; }
    }
}