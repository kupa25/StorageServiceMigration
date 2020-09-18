using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class CreateInvoiceRequest
    {
        public IEnumerable<int> BillableItemIds { get; set; }
        public int BillToId { get; set; }
        public string BillToType { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}