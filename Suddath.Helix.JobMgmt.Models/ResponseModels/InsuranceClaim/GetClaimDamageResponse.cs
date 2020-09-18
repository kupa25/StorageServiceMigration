using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim
{
    public class GetClaimDamageResponse
    {
        public int Id { get; set; }
        public int ServiceOrderClaimId { get; set; }
        public string DamageType { get; set; }
        public string DamageDetails { get; set; }
        public decimal? Cost { get; set; }
    }
}
