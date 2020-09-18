using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class AuthorizedServicesMoveDto
    {
        public ICollection<AuthorizedServiceDto> AuthorizedServices { get; set; }
    }

    public class AuthorizedServiceDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Type { get; set; } //Air,Sea, etc
        public int? SitDays { get; set; }

        public ICollection<AuthorizedServiceDetailsDto> AuthorizedServiceDetails { get; set; }
    }

    public class AuthorizedServiceDetailsDto
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public string MeasurementType { get; set; }
    }
}