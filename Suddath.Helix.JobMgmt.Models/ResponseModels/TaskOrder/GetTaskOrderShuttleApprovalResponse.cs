using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderShuttleApprovalResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Status { get; set; }
        public string ShipmentNumber { get; set; }
        public string ServiceCode { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
    }
}