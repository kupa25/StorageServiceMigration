using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetBillableItemResponse
    {
        public int Id { get; set; }
        public int SuperServiceOrderId { get; set; }
        public string Status { get; set; }
        public int? BillableItemTypeId { get; set; }
        public string BillableItemTypeName { get; set; }
        public string BillToType { get; set; }
        public int? BillToId { get; set; }
        public string BillToName { get; set; }
        public string BillToLabel { get; set; }
        public string BillingCurrency { get; set; }
        public string BillingCurrencySymbol { get; set; }
        public bool IsOriginalAccrual { get; set; }
        public string Description { get; set; }
        public decimal? OriginalAccrualAmountBillingCurrency { get; set; }
        public decimal? OriginalAccrualAmountUSD { get; set; }
        public decimal? AccrualAmountBillingCurrency { get; set; }
        public decimal? AccrualAmountUSD { get; set; }
        public decimal? AccrualExchangeRate { get; set; }
        public DateTime? AccrualExchangeRateDateTime { get; set; }
        public decimal? ActualAmountBillingCurrency { get; set; }
        public decimal? ActualAmountUSD { get; set; }
        public decimal? ActualExchangeRate { get; set; }
        public DateTime? ActualExchangeRateDateTime { get; set; }
        public DateTime? ActualFinancialPeriodDateTime { get; set; }

        public DateTime? ActualPostedDateTime { get; set; }
        public int? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceType { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? AccrualFinancialPeriodDateTime { get; set; }
        public DateTimeOffset? AccrualPostedDateTime { get; set; }
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