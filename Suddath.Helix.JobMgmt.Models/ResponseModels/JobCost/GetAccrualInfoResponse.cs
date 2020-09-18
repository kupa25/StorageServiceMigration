using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetAccrualInfoResponse
    {
        public string AccrualStatus { get; set; }
        public DateTime? AccrualPendingDateTime { get; set; }
        public DateTime? AccrualPostedDateTime { get; set; }
        public string SuperServiceOrderStatus { get; set; }
    }
}