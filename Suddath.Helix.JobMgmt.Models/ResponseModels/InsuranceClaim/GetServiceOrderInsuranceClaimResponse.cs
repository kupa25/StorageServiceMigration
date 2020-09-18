using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim
{
    public class GetServiceOrderInsuranceClaimResponse : ServiceOrderBaseResponse
    {
        public int Id { get; set; }

        //Insurance
        public string InsuranceCarrierName { get; set; }

        public string Declaration { get; set; }
        public string InsuranceType { get; set; }
        public decimal? HHGAmount { get; set; }
        public decimal? HighValueAmount { get; set; }
        public decimal? VehicleAmount { get; set; }
        public decimal? TotalInsuranceAmount { get; set; }
        public decimal? DeductibleAmount { get; set; }
        public new decimal? QuotedRate { get; set; }
        public decimal? PayableRate { get; set; }
        public string PayableRateType { get; set; }
        public bool CanIssuePolicy { get; set; }

        //Claim
        public string ClaimStatus { get; set; }

        public string ClaimNumber { get; set; }
        public DateTime? InactivityClosedDate { get; set; }
        public DateTime? ClaimDate { get; set; }
        public decimal? ClaimedAmount { get; set; }
        public DateTime? ClaimantDocsCreatedDate { get; set; }
        public DateTime? AssigneePaidDate { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? GoodwillAmount { get; set; }
    }
}