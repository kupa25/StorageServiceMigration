using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models.RequestModels.JobCost;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IAdjustmentService
    {
        Task<int> CreateBillableItemAdjustmentAsync(int billableItemId, string adjustmentType, CreateAdjustmentRequest request);

        Task<string> ValidateCreateBillableItemAdjustmentAsync(int billableItemId, string adjustmentType, CreateAdjustmentRequest request);

        Task<bool> GetExistsBillableItemAdjustmentByTupleAsync(int jobId, int superServiceOrderId, int billableItemId, int id);

        Task<bool> GetExistsPayableItemAdjustmentByTupleAsync(int jobId, int superServiceOrderId, int payableItemId, int id);

        Task<int> CreatePayableItemAdjustmentAsync(int payableItemId, string adjustmentType, CreateAdjustmentRequest request);

        Task<string> ValidateCreatePayableItemAdjustmentAsync(int payableItemId, string adjustmentType, CreateAdjustmentRequest request);
    }
}