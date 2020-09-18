using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class CreateInvoiceResponse
    {
        public int BillableRecordsUpdated { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string VendorShortAddress { get; set; }
        public string VendorAccountingId { get; set; }

        public int ServiceOrderId { get; set; }
        public string SuperServiceName { get; set; }
        public string DisplayId { get; set; }
        public string BillToType { get; set; }
        public int? BillToId { get; set; }
        public string BillToName { get; set; }
    }
}