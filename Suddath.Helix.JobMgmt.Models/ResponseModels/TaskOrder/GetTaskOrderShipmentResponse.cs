using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderShipmentResponse
    {
        public int Id { get; set; }
        public string DisplayId { get; set; }
        public int SuperServiceId { get; set; }
        public string SuperServiceName { get; set; }
        public string SuperServiceModeName { get; set; }
        public string SuperServiceIconName { get; set; }
        public string Status { get; set; }
        public IEnumerable<GetTaskOrderShipmentServiceResponse> ServiceOrders { get; set; }
    }

    public class GetTaskOrderShipmentServiceResponse
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAbbreviation { get; set; }
    }
}