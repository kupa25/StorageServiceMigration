using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class CreateVendorInvoiceCreditMemoResponse
    {
        public int PayableRecordsCreated { get; set; }
        public int CreditMemoVendorInvoiceId { get; set; }
    }
}