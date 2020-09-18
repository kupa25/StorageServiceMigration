using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helix.API.Results;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IBillableItemService
    {
        Task<IEnumerable<GetBillableItemResponse>> GetBillableItemsAsync(int superServiceOrderId);

        Task<GetBillableItemResponse> GetBillableItemByIdAsync(int id);

        Task<List<GetBillableItemResponse>> GetBillableItemsByIdsAsync(IEnumerable<int> ids);

        Task<CreateBillableItemResponse> CreateBillableItemAsync(int superServiceOrderId);

        Task UpdateBillableItemAsync(int id, JsonPatchDocument patch);

        Task<string> ValidateUpdateBillableItemAsync(int id, JsonPatchDocument patch);

        Task DeleteBillableItemAsync(int id);

        Task<string> ValidateDeleteBillableItemAsync(int id);

        Task<bool> GetExistsBillableItemByTuple(int jobId, int superServiceOrderId, int id);

        Task<IEnumerable<GetBillableItemTypeResponse>> GetBillableItemTypesAsync();

        Task<IEnumerable<GetSuperServiceOrderAvailableBillTosResponse>> GetSuperServiceOrderAvailableBillTosAsync(int superServiceOrderId);

        Task<LockAccrualsResponse> LockAccrualsAsync(int superServiceOrderId);

        Task<UnlockAccrualsResponse> UnlockAccrualsAsync(int superServiceOrderId);

        Task<string> ValidateLockAccrualsAsync(int superServiceOrderId);

        Task<string> ValidateUnlockAccrualsAsync(int superServiceOrderId);

        Task<IEnumerable<GetPostableResponse>> GetBillableItemAccruablesAsync(int superServiceOrderId, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetBillableItemAccruableByIdAsync(int id, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetBillableItemAccrualAdjustableByIdAsync(int id, DateTime? actualPackEndDate);

        Task<string> ValidateBillableItemAccrualByIdAsync(int id);

        Task<PostedAccrualsResponse> MarkAccrualsPostedAsync(int superServiceOrderId, IEnumerable<int> BillableItemIds, DateTime financialPeriodDateTime, bool isOriginalAccrual);

        Task<PostedAccrualsResponse> MarkBillableItemPostedAsync(int billableItemId, DateTime financialPeriodDateTime);

        Task<PagedResults<GetAccruableBatchResponse>> GetAccruableBatches(string sortBy, int pageNumber = 1, int pageSize = 10, string filter = null, bool descending = false);

        Task<VoidedAccrualsResponse> MarkAccrualsVoidedAsync(int superServiceOrderId, UpdateAccruablesRequest request);

        Task<string> ValidateBillableItemVoidableByIdAsync(int id);

        Task<PostedActualsResponse> MarkActualsAsPostedAsync(int superServiceOrderId, IEnumerable<int> BillableItemIds, DateTime financialPeriodDateTime);

        Task<PostedAdjustmentsResponse> MarkAdjustmentAsPostedAsync(int id);

        Task<IEnumerable<GetReversableResponse>> GetBillableItemAccrualReversalsAsync(ICollection<int> billableItemIds);
    }
}