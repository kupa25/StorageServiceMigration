using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim
{
    public class GetClaimRepairResponse
    {
        public int Id { get; set; }
        public int ServiceOrderClaimId { get; set; }
        public DateTime? RepairDate { get; set; }
        public decimal? Cost { get; set; }
    }
}
