using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface ISuperServiceOrderService
    {
        Task<GetJobCostMetricsResponse> GetJobCostMetricsAsync(int superServiceOrderId);

        Task<bool> IsValidSuperServiceOrderAsync(int jobId, int superServiceOrderId);

        Task<GetAccrualInfoResponse> GetAccrualInfoAsync(int superServiceOrderId);

        Task<DateTime?> GetActualPackEndDateAsync(int superServiceOrderId);

        Task<bool> GetSuperServiceOrderExistsAsync(int superServiceOrderId);

        Task PatchSuperServiceOrderAsync(int superServiceOrderId, JsonPatchDocument<PatchSuperServiceOrderRequest> patch);

        Task<string> ValidateSSOPatchAsync(int superServiceOrderId, JsonPatchDocument<PatchSuperServiceOrderRequest> patch);

        Task<bool> IsInActionableStatus(int superServiceOrderId);

        Task<ICollection<GetTaskOrderShipmentResponse>> GetSuperServiceOrdersForTaskOrderAsync(int jobId);

        Task<IEnumerable<GetSuperServiceOrderAvailableBillTosResponse>> GetSuperServiceOrderAvailableBillTosAsync(int superServiceOrderId);

        Task<int> GetServiceOrderIdAsync(string serviceAbbreviation, int superServiceOrderId);
    }
}