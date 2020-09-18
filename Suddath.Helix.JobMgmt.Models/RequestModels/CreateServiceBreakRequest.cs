using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateServiceBreakRequest
    {
        public bool Complement { get; set; }
        public int AgentID { get; set; }
        public DateTime DateEntered { get; set; }
        public string EnteredBy { get; set; }
        public bool ReportToAgent { get; set; }
        public string Issue { get; set; }
        public string Resolution { get; set; }
        public bool? Resolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }
}
