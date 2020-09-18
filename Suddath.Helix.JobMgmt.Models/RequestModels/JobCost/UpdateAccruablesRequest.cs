using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class UpdateAccruablesRequest
    {
        public IEnumerable<int> BillableItemIds { get; set; }

        public IEnumerable<int> PayableItemIds { get; set; }

        public DateTime EffectiveDateTime { get; set; }

        public bool IsOriginalAccrual { get; set; }
    }
}