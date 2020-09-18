using System.Collections.Generic;
using System.Threading.Tasks;
using Helix.API.Results;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IJobService
    {
        Task<TransfereeFlatDto> GetTransfereeByJobId(int jobId);

        Task<GetTaskOrderMemberInfoResponse> GetTaskOrderTransfereeByJobId(int jobId);

        Task<ICollection<TransfereePortalDto>> GetJobsByAssigneeEmailAsync(string email);

        Task<int> AddJobAsync(Job2Dto job2Dto);

        Task<bool> GetJobExistsAsync(int jobId);

        Task PatchJobAsync(int jobId, JsonPatchDocument patch);

        Task UpdateJobAsync(Job2Dto dto);

        Task<PagedResults<GetJobsResponse>> ListJobsAsync(int pageNumber = 1, int pageSize = 10, string filter = "", string sortBy = "JobId", bool descending = true, bool showAll = true);

        Task<ICollection<Job2Dto>> ListTaskOrdersAsync(string filter = "", string sortBy = "JobId", bool descending = true, bool showAll = true);

        Task<PagedResults<GetJobsSearchIndexResponse>> GetJobsForSearchIndexAsync(int pageNumber = 1, int pageSize = 50, int lookbackMonths = 9);

        Task<Job2Dto> GetJobDetailById(int jobId);

        Task<ICollection<GetSuperServiceOrderResponse>> GetSuperServiceOrdersAsync(int jobId);

        Task<int> CreateSuperServiceOrderAsync(int jobId, int superServiceId, int? superServiceModeId);

        Task<CreateSuperServiceOrderResponse> GetSuperServiceOrderByIdAsync(int superServiceOrderId);

        Task<ICollection<CreateServiceOrderResponse>> CreateTemplateServiceOrdersAsync(int superServiceOrderId);

        Task<ICollection<JobContactDto>> CreateJobContactsAsync(int jobId, ICollection<CreateJobContactDto> contacts);

        Task<ICollection<JobContactDto>> GetJobContactsAsync(int jobId);

        Task PatchJobContactsAsync(int id, JsonPatchDocument patch);

        Task UpdateJobContactsAsync(int id, ICollection<JobContactDto> contacts);

        Task<string> ValidateUpdateJobPatchAsync(int jobId, JsonPatchDocument patch);

        Task<int> SaveAddressAsync(AddressDto address, string type);
    }
}