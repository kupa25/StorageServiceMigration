using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class CreateVendorInvoiceRequest
    {
        public IEnumerable<int> PayableItemIds { get; set; }
        public string BillFromType { get; set; }
        public int BillFromId { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public DateTime VendorInvoiceDate { get; set; }
    }
}