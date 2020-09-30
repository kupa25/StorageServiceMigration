using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class CreateJobNoteRequest
    {
        public int JobId { get; set; }
        public int ReferenceId { get; set; }
        public string CreatedBy { get; set; }
        public string Category { get; set; }
        public string Module { get; set; }
        public string Message { get; set; }
        public bool IsCritical { get; set; } = false;
        public string AssignedTo { get; set; }
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Add the Id that the user would want to see to know where the note was added from.
        /// </summary>
        public string DisplayId { get; set; }
    }
}