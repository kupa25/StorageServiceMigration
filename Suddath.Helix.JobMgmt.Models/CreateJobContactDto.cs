using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class CreateJobContactDto
    {
        [Required]
        public string ContactType { get; set; }

        [Required]
        public string FullName { get; set; }

        public string PhoneCountryCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExtension { get; set; }

        [Required]
        public string Email { get; set; }
    }
}