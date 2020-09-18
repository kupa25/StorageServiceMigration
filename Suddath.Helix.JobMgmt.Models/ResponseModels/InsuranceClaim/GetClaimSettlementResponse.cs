using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim
{
    public class GetClaimSettlementResponse
    {
        public int Id { get; set; }
        public int ServiceOrderInsuranceClaimId { get; set; }
        public DateTime? SettlementOfferedDate { get; set; }
        public decimal? SettlementOfferedAmount { get; set; }
        public DateTime? SettlementAcceptedDate { get; set; }
    }
}