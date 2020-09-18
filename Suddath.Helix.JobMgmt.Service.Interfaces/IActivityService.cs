using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.RequestModels.Activity;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interface
{
    public interface IActivityService
    {
        /// <summary>
        /// Create or update an Activity and insert a new ActivityDetail record.
        /// </summary>
        /// <param name="jobId">The Job Identifier</param>
        /// <param name="superServiceOrderId">The ServiceOrder Identifier</param>
        /// <param name="dto">CreateActivityDetailRequest object</param>
        /// <returns>new activity detail id</returns>
        Task<int> HandleNewActivityDetailAsync(int jobId, int? superServiceOrderId, CreateActivityDetailRequest dto);

        Task<int> CreateShipmentActivity(int jobId, int superServiceOrderId, CreateActivityDetailRequest dto);

        Task<IEnumerable<ActivityDetailDto>> GetTaskOrderActivities(int jobId);

        Task<GetTaskOrderActivityResponse> GetTaskOrderActivityAsync(int jobId, string typeCode);

        /// <summary>
        /// Creates an Activity record for a TaskOrder Activity and the associated TaskOrderActivity relationship row
        /// </summary>
        /// <param name="jobId">The TaskOrder's Id</param>
        /// <param name="activityDetailRequest">The ActivityDetail information</param>
        /// <returns>ActivityId</returns>
        Task<int> CreateTaskOrderActivityRecord(int jobId, CreateActivityDetailRequest activityDetailRequest);

        /// <summary>
        /// Creates an ActivityDetail record for an associated Activity record.
        /// </summary>
        /// <param name="activityDetail">The ActivityDetail entity</param>
        /// <returns>Id of new ActivityDetail</returns>
        Task<int> CreateActivityDetailRecord(ActivityDetailDto activityDetail);

        Task<IEnumerable<int>> CompleteApprovedServiceItemsForActivityAsync(int activityDetailId);
    }
}