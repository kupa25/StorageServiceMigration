using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class CreateVendorInvoiceResponse
    {
        public int PayableRecordsUpdated { get; set; }
        public int VendorInvoiceId { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public string VendorShortAddress { get; set; }
        public string VendorAccountingId { get; set; }
        public int ServiceOrderId { get; set; }
        public string SuperServiceName { get; set; }
        public string DisplayId { get; set; }
        public string BillFromType { get; set; }
        public int? BillFromId { get; set; }
        public string BillFromName { get; set; }
    }
}