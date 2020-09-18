using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class LockAccrualsResponse
    {
        public int RecordsUpdated { get; set; }
        public DateTime AccrualPendingDateTime { get; set; }
    }
}