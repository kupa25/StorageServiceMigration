using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class TaskOrderAddressInfoResponse
    {
        public HfAddressDto OriginAddress { get; set; }
        public HfAddressDto DestinationAddress { get; set; }
    }
}