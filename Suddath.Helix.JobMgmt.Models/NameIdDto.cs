using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class NameIdDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0")]
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
