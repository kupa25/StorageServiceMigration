using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class CreateInvoiceCreditMemoResponse
    {
        public int BillableRecordsCreated { get; set; }
        public int CreditMemoInvoiceId { get; set; }
    }
}