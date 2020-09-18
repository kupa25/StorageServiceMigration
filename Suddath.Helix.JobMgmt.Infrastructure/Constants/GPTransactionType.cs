using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class GPTransactionTypeIdentifier
    {
        public const string POST_ACCRUAL_BILLABLE_ITEM = "POST_ACCRUAL_BILLABLE_ITEM";
        public const string POST_ACCRUAL_PAYABLE_ITEM = "POST_ACCRUAL_PAYABLE_ITEM";
        public const string REVERSE_ACCRUAL_BILLABLE_ITEM = "REVERSE_ACCRUAL_BILLABLE_ITEM";
        public const string REVERSE_ACCRUAL_PAYABLE_ITEM = "REVERSE_ACCRUAL_PAYABLE_ITEM";
        public const string POST_ACTUAL_INVOICE = "POST_ACTUAL_INVOICE";
        public const string POST_ACTUAL_VENDOR_INVOICE = "POST_ACTUAL_VENDOR_INVOICE";
        public const string REVERSE_ACTUAL_INVOICE = "REVERSE_ACTUAL_INVOICE";
        public const string REVERSE_ACTUAL_VENDOR_INVOICE = "REVERSE_ACTUAL_VENDOR_INVOICE";
    }
}