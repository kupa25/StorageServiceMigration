using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public partial class Note
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        public int? TaskId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public string Module { get; set; }
        public bool IsCritical { get; set; }
        public string DisplayId { get; set; }
    }
}