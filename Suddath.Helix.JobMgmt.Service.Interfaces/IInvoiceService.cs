using System.Collections.Generic;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models.RequestModels.JobCost;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IInvoiceService
    {
        #region Existence Checks

        Task<bool> GetExistsInvoiceByTupleAsync(int jobId, int superServiceOrderId, int invoiceId);

        Task<bool> GetExistsVendorInvoiceByTupleAsync(int jobId, int superServiceOrderId, int vendorInvoiceId);

        #endregion Existence Checks

        #region Invoices

        Task<CreateInvoiceResponse> CreateInvoiceAsync(int superServiceOrderId, CreateInvoiceRequest request);

        Task<string> ValidateCreateInvoiceAsync(int superServiceOrderId, CreateInvoiceRequest request);

        Task<IEnumerable<GetBillableItemResponse>> GetBillableItemsByInvoiceIdAsync(int invoiceId);

        #endregion Invoices

        #region VendorInvoices

        Task<CreateVendorInvoiceResponse> CreateVendorInvoiceAsync(int superServiceOrderId, CreateVendorInvoiceRequest request);

        Task<string> ValidateCreateVendorInvoiceAsync(int superServiceOrderId, CreateVendorInvoiceRequest request);

        Task<IEnumerable<GetPayableItemResponse>> GetPayableItemsByVendorInvoiceIdAsync(int vendorInvoiceId);

        #endregion VendorInvoices

        #region Invoice CreditMemos

        Task<int> CreateInvoiceCreditMemoAsync(int invoiceId, CreateInvoiceCreditMemoRequest request);

        Task<int> CreateInvoiceCreditMemoBookingCommissionItemAsync(int creditMemoInvoiceId, decimal itemAmountUSD);

        Task<int> CreateVendorInvoiceCreditMemoBookingCommissionItemAsync(int creditMemoInvoiceId, decimal itemAmountUSD);

        Task<CreateInvoiceCreditMemoResponse> CreateBillableItemsForCreditMemoAsync(int creditMemoInvoiceId, IEnumerable<CreateItemCreditMemoRequest> billableItemsToClone);

        Task<string> ValidateCreateInvoiceCreditMemoAsync(int invoiceId, CreateInvoiceCreditMemoRequest request);

        #endregion Invoice CreditMemos

        #region VendorInvoice CreditMemos

        Task<int> CreateVendorInvoiceCreditMemoAsync(int vendorInvoiceId, CreateVendorInvoiceCreditMemoRequest request);

        Task<CreateVendorInvoiceCreditMemoResponse> CreatePayableItemsForCreditMemoAsync(int creditMemoVendorInvoiceId, IEnumerable<CreateItemCreditMemoRequest> request);

        Task<string> ValidateCreateVendorInvoiceCreditMemoAsync(int vendorInvoiceId, CreateVendorInvoiceCreditMemoRequest request);

        #endregion VendorInvoice CreditMemos
    }
}