using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateFeedbackDto
    {
        public string Name { get; set; }
        public string Email { get; set; }

        [Range(1, 5, ErrorMessage = "Range must be between 1 and 5")]
        [Required]
        public int Rating { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Message is too long, please shorten it.")]
        public string Message { get; set; }

        [Required]
        public string Module { get; set; }

        public string Url { get; set; }
    }
}