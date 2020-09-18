using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IPayableItemService
    {
        Task<IEnumerable<GetPayableItemResponse>> GetPayableItemsAsync(int superServiceOrderId);

        Task<CreatePayableItemResponse> CreatePayableItemAsync(int superServiceOrderId);

        Task UpdatePayableItemAsync(int id, JsonPatchDocument patch);

        Task<string> ValidateUpdatePayableItemAsync(int id, JsonPatchDocument patch);

        Task DeletePayableItemAsync(int id);

        Task<string> ValidateDeletePayableItemAsync(int id);

        Task<bool> GetExistsPayableItemByTuple(int jobId, int superServiceOrderId, int id);

        Task<IEnumerable<GetBillableItemTypeResponse>> GetPayableItemTypesAsync();

        Task<IEnumerable<GetSuperServiceOrderAvailableBillFromResponse>> GetSuperServiceOrderAvailableBillFromAsync(int superServiceOrderId);

        Task<LockAccrualsResponse> LockAccrualsAsync(int superServiceOrderId);

        Task<UnlockAccrualsResponse> UnlockAccrualsAsync(int superServiceOrderId);

        Task<string> ValidateLockAccrualsAsync(int superServiceOrderId);

        Task<string> ValidateUnlockAccrualsAsync(int superServiceOrderId);

        Task<IEnumerable<GetPostableResponse>> GetPayableItemAccruablesAsync(int superServiceOrderId, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetPayableItemAccruableByIdAsync(int id, DateTime? actualPackEndDate);

        Task<string> ValidatePayableItemAccrualByIdAsync(int id);

        Task<PostedAccrualsResponse> MarkAccrualsPostedAsync(int superServiceOrderId, IEnumerable<int> PayableItemIds, DateTime accrualFinancialPeriodDateTime, bool isOriginalAccrual);

        Task<PostedAccrualsResponse> MarkPayableItemPostedAsync(int payableItemId, DateTime financialPeriodDateTime);

        Task<VoidedAccrualsResponse> MarkAccrualsVoidedAsync(int superServiceOrderId, UpdateAccruablesRequest request);

        Task<PostedActualsResponse> MarkActualsAsPostedAsync(int superServiceOrderId, IEnumerable<int> PayableItemIds, DateTime accrualFinancialPeriodDateTime);

        Task<string> ValidatePayableItemVoidableByIdAsync(int id);

        Task<GetPostableResponse> GetPayableItemAdjustableByIdAsync(int id, DateTime? actualPackEndDate);

        Task<IEnumerable<GetReversableResponse>> GetPayableItemReversalsAsync(ICollection<int> payableItemIds);

        Task<PostedAdjustmentsResponse> MarkAdjustmentAsPostedAsync(int id);
    }
}