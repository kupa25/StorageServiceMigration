using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class CreateVendorInvoiceCreditMemoRequest
    {
        public IEnumerable<CreateItemCreditMemoRequest> PayableItems { get; set; }
        public string Explanation { get; set; }
        public decimal? BookingCommission1Amount { get; set; }
        public decimal? BookingCommission2Amount { get; set; }
        public decimal? BookingCommission3Amount { get; set; }
    }
}