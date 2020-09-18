using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class Job
    {
        [NotMapped]
        public string BillToLabel { get; set; }
    }
}