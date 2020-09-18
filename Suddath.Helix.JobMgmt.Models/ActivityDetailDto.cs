using System;

namespace Suddath.Helix.JobMgmt.Models
{
    public class ActivityDetailDto
    {
        public int? Id { get; set; }
        public int ActivityId { get; set; }
        public int? AssignedProviderVendorId { get; set; }

        public string ActivityDetailType { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}

