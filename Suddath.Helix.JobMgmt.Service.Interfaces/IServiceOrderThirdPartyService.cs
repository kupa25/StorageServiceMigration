using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderThirdParty;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IServiceOrderThirdPartyService
    {
        #region services

        Task<bool> GetExistsTPServiceByTupleAsync(int jobId, int serviceOrderId, int id);

        Task<IEnumerable<GetThirdPartyServiceResponse>> GetThirdPartyServicesAsync(int serviceOrderId);

        Task<int> CreateThirdPartyServiceAsync(int serviceOrderId);

        Task PatchThirdPartyServiceAsync(int id, JsonPatchDocument patch);

        Task UpdateThirdPartyServiceAsync(int id, GetThirdPartyServiceResponse dto);

        Task DeleteThirdPartyServiceAsync(int id);

        #endregion services

        #region crates

        Task<bool> GetExistsTPCrateByTupleAsync(int jobId, int serviceOrderId, int id);

        Task<IEnumerable<GetThirdPartyCrateResponse>> GetThirdPartyCratesAsync(int serviceOrderId);

        Task<int> CreateThirdPartyCrateAsync(int serviceOrderId);

        Task PatchThirdPartyCrateAsync(int id, JsonPatchDocument patch);

        Task UpdateThirdPartyCrateAsync(int id, GetThirdPartyCrateResponse dto);

        Task DeleteThirdPartyCrateAsync(int id);

        #endregion crates
    }
}