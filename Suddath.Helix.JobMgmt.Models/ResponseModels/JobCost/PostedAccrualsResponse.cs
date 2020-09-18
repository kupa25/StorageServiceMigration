using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class PostedAccrualsResponse
    {
        public int BillableRecordsUpdated { get; set; }

        public int PayableRecordsUpdated { get; set; }
        public DateTime AccrualPostedDateTime { get; set; }
    }
}