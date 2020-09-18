using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Models.Configuration
{
    public class EmailOptions
    {
        public string SubmitMessage { get; set; }
        public string SendGridApiKey { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string DebugEmail { get; set; }
        public bool UseDebugEmail { get; set; }
    }
}
