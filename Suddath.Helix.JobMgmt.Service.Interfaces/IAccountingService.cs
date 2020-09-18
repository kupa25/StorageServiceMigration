using System.Threading.Tasks;
using Helix.API.Results;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Accounting;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IAccountingService
    {
        Task<PagedResults<GetGLCodeResponse>> GetGLCodesAsync(string AccountCode, string AccrualOrActual, string ReceivableOrPayable, string BranchName, string RevenueType, int? GPDistType, int skip, int take);

        Task<string> GetGLCodeAsync(string accrualOrActual, string receivableOrPayable, string branchName, string revenueType, string accountCode, int gPDistType);

        Task GenerateGPDocNumsForSuperServiceOrderAsync(int superServiceOrderId);

        Task GenerateGPDocNumsForBillableItemAsync(int superServiceOrderId, int billableItemId);

        Task GenerateGPDocNumsForPayableItemAsync(int superServiceOrderId, int payableItemId);
    }
}