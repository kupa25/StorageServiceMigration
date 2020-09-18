using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderShipmentInfoResponse
    {
        public int Id { get; set; }
        public string ShipmentName { get; set; }
        public string ShipmentIconName { get; set; }
        public string ShipmentIdentifier { get; set; }
        public string ShipmentType { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        public DateTime? RequestedPickupDateTime { get; set; }
        public DateTime? RequestedDeliveryDateTime { get; set; }
        public DateTime? RequiredDeliveryDateTime { get; set; }
        public decimal? EstimatedWeightLb { get; set; }
        public AddressDto PickupAddress { get; set; }
        public AddressDto DestinationAddress { get; set; }
        public string Remark { get; set; }
    }
}