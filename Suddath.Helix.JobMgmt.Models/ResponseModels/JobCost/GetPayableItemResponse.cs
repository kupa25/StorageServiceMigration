using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetPayableItemResponse
    {
        public int Id { get; set; }
        public int SuperServiceOrderId { get; set; }
        public string Status { get; set; }
        public int? PayableItemTypeId { get; set; }
        public string PayableItemTypeName { get; set; }
        public string BillFromType { get; set; }
        public string BillFromName { get; set; }
        public int? BillFromId { get; set; }
        public string BillFromLabel { get; set; }
        public string VendorName { get; set; }
        public string VendorCurrency { get; set; }
        public string VendorCurrencySymbol { get; set; }
        public bool IsOriginalAccrual { get; set; }
        public string Description { get; set; }
        public decimal? AccrualAmountVendorCurrency { get; set; }
        public decimal? AccrualAmountUSD { get; set; }
        public decimal? AccrualExchangeRate { get; set; }
        public DateTime? AccrualExchangeRateDateTime { get; set; }
        public decimal? OriginalAccrualAmountVendorCurrency { get; set; }
        public decimal? OriginalAccrualAmountUSD { get; set; }
        public DateTime? AccrualFinancialPeriodDateTime { get; set; }
        public DateTimeOffset? AccrualPostedDateTime { get; set; }
        public string AccrualPostedBy { get; set; }
        public decimal? ActualAmountVendorCurrency { get; set; }
        public decimal? ActualAmountUSD { get; set; }
        public decimal? ActualExchangeRate { get; set; }
        public DateTime? ActualExchangeRateDateTime { get; set; }
        public DateTime? ActualFinancialPeriodDateTime { get; set; }

        public DateTime? ActualPostedDateTime { get; set; }
        public string ActualPostedBy { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public int? VendorInvoiceId { get; set; }
        public string VendorInvoiceType { get; set; }
        public DateTime? VendorInvoiceDate { get; set; }

        public string CheckWireNumber { get; set; }
        public DateTime? PaidDate { get; set; }
        public string HoldType { get; set; }
        public decimal? WriteOffAmountUSD { get; set; }
        public string WriteOffReason { get; set; }
        public string WriteOffExplanation { get; set; }
        public string WriteOffModifiedBy { get; set; }
        public DateTimeOffset? WriteOffModifiedDateTime { get; set; }
        public bool IsAccrualAdjusted { get; set; }
        public string AccrualAdjustmentBy { get; set; }
        public DateTime? AccrualAdjustmentDateTime { get; set; }
        public bool IsActualAdjusted { get; set; }
        public string ActualAdjustmentBy { get; set; }
        public DateTime? ActualAdjustmentDateTime { get; set; }
        public string VoidedBy { get; set; }
        public DateTimeOffset? VoidedDateTime { get; set; }
        public DateTime? VoidedFinancialPeriodDate { get; set; }
    }
}