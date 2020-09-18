using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetFeedbackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public string Message { get; set; }
        public string Module { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
    }
}
