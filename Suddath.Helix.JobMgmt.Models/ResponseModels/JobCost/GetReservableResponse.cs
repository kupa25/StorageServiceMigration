using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetReversableResponse
    {
        public string DocNum { get; set; }
        public int? BillableItemId { get; set; }
        public int? PayableItemId { get; set; }
        public int? InvoiceId { get; set; }
        public int? VendorInvoiceId { get; set; }
    }
}