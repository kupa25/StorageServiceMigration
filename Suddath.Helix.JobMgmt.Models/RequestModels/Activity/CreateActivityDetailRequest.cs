using System;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.Activity
{
    public class CreateActivityDetailRequest
    {
        public int? AssignedProviderVendorId { get; set; }

        public string ActivityTypeCode { get; set; }
        public string ActivityDetailType { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}