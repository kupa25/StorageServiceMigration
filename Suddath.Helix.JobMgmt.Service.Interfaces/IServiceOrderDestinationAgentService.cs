using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderDestinationAgent;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IServiceOrderDestinationAgentService
    {
        Task<bool> GetExistsDAPartialPartiesByTupleAsync(int jobId, int serviceOrderId, int id);

        Task<int> CreateDestinationAgentPartialDelivery(int serviceOrderId);

        Task<IEnumerable<GetDestinationAgentPartialDeliveryResponse>> GetDestinationAgentPartialDeliveries(int serviceOrderId);

        Task PatchDestinationAgentPartialDeliveryAsync(int id, JsonPatchDocument patch);

        Task UpdateDestinationAgentPartialDeliveryAsync(int id, GetDestinationAgentPartialDeliveryResponse dto);

        Task DeleteDestinationAgentPartialDeliveryAsync(int id);
    }
}