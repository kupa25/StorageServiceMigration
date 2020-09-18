using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class GPDistType
    {
        public const int INVOICE = 1;
        public const int PAYABLE = 2;
        public const int RECEIVABLE = 3;
        public const int PURCHASE = 6;
        public const int SALES = 9;
        public const int RETURN = 17;
        public const int DEBIT_MEMO = 18;
        public const int CREDIT_MEMO = 19;
    }
}