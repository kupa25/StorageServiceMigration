using Helix.API.Results;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.ServiceItemAdjudicated;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.ServiceMemberProfileUpdated;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.ShipmentAdded;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.HomeFront;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interface
{
    public interface ITaskOrderService
    {
        Task<PagedResults<TaskOrderResponse>> GetTaskOrdersAsync(
            string filter,
            string sortBy = "TaskOrderId",
            int pageNumber = 1,
            int pageSize = 10,
            bool descending = false);

        Task<(bool exists, int? jobId)> GetTaskOrderExistsAsync(string taskOrderIdentifier);

        Task<bool> GetShipmentExists(string shipmentIdentifier);

        Task<GetTaskOrderMemberInfoResponse> GetTaskOrderTransfereeByJobId(int jobId);

        Task<TaskOrderResponse> GetTaskOrderAsync(int jobId);

        Task<GetTaskOrderEntitlementResponse> GetTaskOrderEntitlement(int jobId);

        Task CreateTaskOrderForJobAsync(int jobId, TaskOrderDto taskOrderDto);

        Task<int> MergeTaskOrderAsync(TaskOrderDto dto);

        Task<int> CreateSuperServiceOrderFromShipmentAsync(int jobId, ShipmentDto dto);

        Task CreateTaskOrderEntitlementAsync(int jobId, EntitlementDto entitlementDto);

        #region Shipment CRUD

        Task CreateShipmentAsync(int jobId, int superServiceOrderId, ShipmentDto shipmentDto);

        Task PatchShipmentAsync(int superServiceOrderId, JsonPatchDocument patch);

        #endregion Shipment CRUD

        Task<ErrorResponse> ValidateTaskOrderAssignedEventAsync(TaskOrderDto request);

        Task<ErrorResponse> ValidateShipmentExists(int superServiceOrderId);

        Task<IEnumerable<GetTaskOrderShipmentInfoResponse>> GetShipmentInformation(int taskOrderId);

        #region ServiceItems

        Task<bool> GetServiceItemExistsByTupleAsync(int jobId, int id);

        Task CreateServiceItemsFromHomeFrontRequestAsync(int jobId, int? superServiceOrderId, IEnumerable<ServiceItemDto> serviceItemDtos);

        Task<int> CreateApprovalAsync(int jobId, ApprovalDto approvalDto);

        Task PatchApprovalAsync(int approvalId, JsonPatchDocument patch);

        Task<GetTaskOrderApprovalResponse> GetApprovalsForJobAsync(int jobId);

        Task<int> CreateServiceItemApprovalRequestAsync(int serviceItemId);

        Task<bool> ValidateApprovalForSubmission(int serviceItemId);

        Task HandleServiceItemAdjudicationRequestAsync(ServiceItemAdjudicatedEventDto adjudicatedEventDto);

        Task DeleteApprovalAsync(int serviceItemId);

        #endregion ServiceItems

        Task HandleShipmentAddedAsync(string taskOrderIdentifier, ShipmentAddedDto data);

        Task<ErrorResponse> ValidateServiceMember(string taskOrderIdentifier, string serviceMemberIdentifier);

        Task UpdateServiceMemberAsync(ServiceMemberProfileUpdatedEventDto data);
    }
}