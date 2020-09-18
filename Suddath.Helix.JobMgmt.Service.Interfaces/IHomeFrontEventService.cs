using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.Error;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IHomeFrontEventService
    {
        Task PostTaskOrderAssignmentAcknowledgedEventAsync(int jobId);

        Task PublishNewShipmentEvent(int superServiceOrderId);

        Task PublishCoordinatorAssignedEventAsync(int jobContactId);

        Task PublishShipmentWeightUpdatedEventAsync(int superServiceOrderId);

        Task PublishActivityEventAsync(int activityDetailId);

        Task PublishServiceItemRequestedOrResubmittedEventAsync(int approvalRequestId);

        Task PublishShipmentStorageInboundEventAsync(int serviceItemId, int? vendorId, DateTime sitInDate);

        Task PublishServiceItemsCompletedEventAsync(IEnumerable<int> serviceItemDetailIds);

        Task PublishErrorNotificationEventAsync(string taskOrderIdentifier, string payload, IEnumerable<ErrorDetailDto> errorList);

        Task PublishPaymentRequestedEventAsync(int paymentRequestId);

        Task PublishShipmentStorageOutboundEventAsync(int serviceItemId, DateTime sitOutDate);
    }
}