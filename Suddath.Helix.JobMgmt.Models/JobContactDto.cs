using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class JobContactDto : CreateJobContactDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
    }
}