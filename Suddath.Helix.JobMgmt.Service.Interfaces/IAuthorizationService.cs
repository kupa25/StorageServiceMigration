using System.Collections.Generic;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Authorization;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface ISuperServiceAuthorization
    {
        Task<IEnumerable<GetSuperServiceAuthorizationResponse>> GetAuthorizations(int jobId);

        Task<GetSuperServiceAuthorizationResponse> CreateSuperServiceAuthorization(int jobId, int superServiceId, CreateSuperServiceAuthorizationRequest request);

        Task<bool> DeleteSuperServiceAuthorization(int jobId, int superServiceId, int id);
    }
}