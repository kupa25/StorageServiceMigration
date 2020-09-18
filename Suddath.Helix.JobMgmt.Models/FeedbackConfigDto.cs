using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class FeedbackConfigDto
    {
        public string Module { get; set; }
        public string Title { get; set; }
        public int StarCount { get; set; }
        public int MsgMinLength { get; set; }
        public int MsgMaxLength { get; set; }
        public int NotificationTime { get; set; }
        public bool IsActive { get; set; }
        public string Url { get; set; }
    }
}