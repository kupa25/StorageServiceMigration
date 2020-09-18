using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    //TODO: This is also used by Gets and patches so maybe create should be removed from name
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