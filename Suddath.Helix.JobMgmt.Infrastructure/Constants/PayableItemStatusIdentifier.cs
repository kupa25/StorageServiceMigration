using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class PayableItemStatusIdentifier
    {
        public static string QUEUED = "QUEUED";
        public static string ACCRUAL_PENDING = "ACCRUAL_PENDING";
        public static string ACCRUAL_POSTED = "ACCRUAL_POSTED";
        public static string VOID = "VOID";
        public static string ACTUAL_PENDING = "ACTUAL_PENDING";
        public static string ACTUAL_POSTED = "ACTUAL_POSTED";
        public static string PARTIAL_PAID = "PARTIAL_PAID";
        public static string CLOSED_PAID = "CLOSED_PAID";
    }
}