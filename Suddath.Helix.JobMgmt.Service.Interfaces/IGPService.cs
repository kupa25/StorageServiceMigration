using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models.RequestModels.JobCost;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IGPService
    {
        #region Get Billable Actuals/Adjustables/CreditMemos

        Task<IEnumerable<GetPostableResponse>> GetActualsByInvoiceIdAsync(int superServiceOrderId, int invoiceId, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetBillableItemActualAdjustableByIdAsync(int id, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetActualsByCreditMemoInvoiceIdAsync(int creditMemoInvoiceId, DateTime? actualPackEndDate);

        Task<GetReversableResponse> GetInvoiceDocNumByCreditMemoInvoiceIdAsync(int creditMemoInvoiceId);

        #endregion Get Billable Actuals/Adjustables/CreditMemos

        #region Get Payable Actuals/Adjustables/CreditMemos

        Task<IEnumerable<GetPostableResponse>> GetActualsByVendorInvoiceIdAsync(int superServiceOrderId, int vendorInvoiceId, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetPayableItemActualAdjustableByIdAsync(int id, DateTime? actualPackEndDate);

        Task<GetPostableResponse> GetActualsByCreditMemoVendorInvoiceIdAsync(int creditMemoVendorInvoiceId, DateTime? actualPackEndDate);

        Task<GetReversableResponse> GetVendorInvoiceDocNumByCreditMemoInvoiceIdAsync(int creditMemoVendorInvoiceId);

        #endregion Get Payable Actuals/Adjustables/CreditMemos

        #region Reversals

        Task<IEnumerable<GetReversableResponse>> GetActualReversalsByBillableItemIdAsync(int billableItemId);

        Task<IEnumerable<GetReversableResponse>> GetPayableItemAccrualReversalsAsync(ICollection<int> payableItemIds);

        Task<GetReversableResponse> GetActualReversalForPayableItemAsync(int payableItemId);

        #endregion Reversals

        #region Transactions

        Task<IEnumerable<CreateGPTransactionResponse>> AddTransaction(int superServiceOrderId, IEnumerable<CreateGPTransactionRequest> request);

        #endregion Transactions
    }
}