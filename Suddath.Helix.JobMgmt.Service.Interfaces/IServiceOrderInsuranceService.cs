using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IServiceOrderInsuranceClaimService
    {
        Task<bool> GetExistsInsuranceClaimByTuple(int jobId, int serviceOrderId, int id);

        Task<IEnumerable<GetServiceOrderInsuranceClaimResponse>> GetInsuranceClaimAsync(int serviceOrderId);

        Task<GetServiceOrderInsuranceClaimResponse> PatchInsuranceClaimAsync(int serviceOrderId, JsonPatchDocument patch);

        Task UpdateInsuranceClaimAsync(int serviceOrderId, GetServiceOrderInsuranceClaimResponse dto);

        Task<bool> GetExistsClaimRepairByTuple(int jobId, int serviceOrderClaimId, int id);

        Task<IEnumerable<GetClaimRepairResponse>> GetClaimRepairsAsync(int serviceOrderClaimId);

        Task<int> CreateClaimRepairAsync(int serviceOrderClaimId);

        Task PatchClaimRepairAsync(int id, JsonPatchDocument patch);

        Task UpdateClaimRepairAsync(int id, GetClaimRepairResponse dto);

        Task DeleteClaimRepairAsync(int id);

        Task<bool> GetExistsClaimSettlementByTuple(int jobId, int serviceOrderInsuranceClaimId, int id);

        Task<IEnumerable<GetClaimSettlementResponse>> GetClaimSettlementAsync(int serviceOrderInsuranceClaimId);

        Task<int> CreateClaimSettlementAsync(int serviceOrderInsuranceClaimId);

        Task PatchClaimSettlementAsync(int id, JsonPatchDocument patch);

        Task UpdateClaimSettlementAsync(int id, GetClaimSettlementResponse dto);

        Task DeleteClaimSettlementAsync(int id);

        Task<bool> GetExistsClaimDamageByTuple(int jobId, int serviceOrderClaimId, int id);

        Task<IEnumerable<GetClaimDamageResponse>> GetClaimDamagesAsync(int serviceOrderClaimId);

        Task<int> CreateClaimDamageAsync(int serviceOrderClaimId);

        Task PatchClaimDamageAsync(int id, JsonPatchDocument patch);

        Task UpdateClaimDamageAsync(int id, GetClaimDamageResponse dto);

        Task DeleteClaimDamageAsync(int id);

        Task<bool> GetExistsClaimInspectionByTuple(int jobId, int serviceOrderClaimId, int id);

        Task<IEnumerable<GetClaimInspectionResponse>> GetClaimInspectionsAsync(int serviceOrderClaimId);

        Task<int> CreateClaimInspectionAsync(int serviceOrderClaimId);

        Task PatchClaimInspectionAsync(int id, JsonPatchDocument patch);

        Task UpdateClaimInspectionAsync(int id, GetClaimInspectionResponse dto);

        Task DeleteClaimInspectionAsync(int id);
    }
}