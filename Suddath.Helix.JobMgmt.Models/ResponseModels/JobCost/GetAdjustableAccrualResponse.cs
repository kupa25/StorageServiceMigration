using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetAdjustableResponse
    {
        public GetPostableResponse Accruable { get; set; }
        public GetReversableResponse Reversable { get; set; }
    }
}