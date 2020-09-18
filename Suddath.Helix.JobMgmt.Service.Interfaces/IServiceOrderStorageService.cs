using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IServiceOrderStorageService
    {
        Task<IEnumerable<GetStorageRevenueResponse>> GetStorageRevenues(int serviceOrderId);

        Task<int> CreateStorageRevenue(int jobId, int serviceOrderId);

        Task PatchStorageRevenueAsync(int id, JsonPatchDocument patch);

        Task<string> ValidateCreateStorageRevenue(int serviceOrderId);
    }
}