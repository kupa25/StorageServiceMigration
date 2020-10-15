using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class CreateJobTaskRequest
    {
        public string Subject { get; set; }
        public string Category { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCritical { get; set; }
        public string AssignedTo { get; set; }
        public string CreatedBy { get; set; }
        public string DisplayId { get; set; }

        public int JobId { get; set; }
        public string TransfereeFirstName { get; set; }
        public string TransfereeLastName { get; set; }
        public bool IsVip { get; set; }
        public string AccountName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}