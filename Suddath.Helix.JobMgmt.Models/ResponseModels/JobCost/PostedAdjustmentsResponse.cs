using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class PostedAdjustmentsResponse
    {
        public int AdjustmentId { get; set; }
        public int BillableRecordsUpdated { get; set; }

        public int PayableRecordsUpdated { get; set; }
        public DateTime AdjustmentPostedDateTime { get; set; }
    }
}