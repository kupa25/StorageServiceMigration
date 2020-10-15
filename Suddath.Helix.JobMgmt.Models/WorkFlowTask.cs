using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public partial class WorkflowTask
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        public string Module { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCritical { get; set; }
        public bool IsCompleted { get; set; }
        public string AssignedTo { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string CompletedBy { get; set; }
        public string DisplayId { get; set; }
        public int? TaskTemplateId { get; set; }
    }
}