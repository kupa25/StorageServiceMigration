using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderServiceItemApprovalResponse
    {
        //Service Item Id
        public int Id { get; set; }

        public int JobId { get; set; }
        public string RequestedServiceCode { get; set; }
        public string SourceSystem { get; set; }
        public int RequestedServiceId { get; set; }
        public string Status { get; set; }
        public string PickupPostalCode { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public DateTime? FirstAvailableDeliveryDateTime { get; set; }
        public DateTime? NextAvailableDeliveryDateTime { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}