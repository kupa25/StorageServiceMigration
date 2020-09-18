using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetSuperServiceOrderStatusResponse
    {
        public string StatusName { get; set; }
        public string DisplayName { get; set; }
    }

    public class GetJobStatusResponse : GetSuperServiceOrderStatusResponse { }
}