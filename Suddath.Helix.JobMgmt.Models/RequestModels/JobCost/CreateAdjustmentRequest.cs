using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class CreateAdjustmentRequest
    {
        public decimal? NewAmount { get; set; }
        public string Explanation { get; set; }
    }
}