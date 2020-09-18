using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetGPInvoiceResponse
    {
        public IEnumerable<GetPostableResponse> Actuals { get; set; }
        public IEnumerable<GetReversableResponse> Reversals { get; set; }
    }
}