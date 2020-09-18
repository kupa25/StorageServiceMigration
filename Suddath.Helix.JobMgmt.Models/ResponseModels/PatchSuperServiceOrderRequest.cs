using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class PatchSuperServiceOrderRequest
    {
        public int? SuperServiceModeId { get; set; }
        public string Status { get; set; }
    }
}