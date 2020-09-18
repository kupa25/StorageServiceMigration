using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim
{
    public class GetClaimInspectionResponse
    {
        public int Id { get; set; }
        public int ServiceOrderClaimId { get; set; }
        public DateTime? InspectionDate { get; set; }
        public decimal? Cost { get; set; }
    }
}
