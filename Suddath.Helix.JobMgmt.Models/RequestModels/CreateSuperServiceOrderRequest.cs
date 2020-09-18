using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateSuperServiceOrderRequest
    {
        public int SuperServiceId { get; set; }
        public int? SuperServiceModeId { get; set; }
    }
}