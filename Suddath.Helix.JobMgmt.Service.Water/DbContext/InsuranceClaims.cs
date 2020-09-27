using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("CLAIMS_INSURANCE")]
    public partial class InsuranceClaims
    {
        [Key]
        public string MOVES_ID { get; set; }

        public string CARRIER { get; set; }
        public decimal? REQ_AMOUNT { get; set; }
        public decimal? HIGH_VALUE_AMOUNT { get; set; }
        public decimal? VEHICLE_AMOUNT { get; set; }
        public decimal? DEDUCTIBLE { get; set; }
        public decimal? PREMIUM_RATE { get; set; }
        public decimal? PREMIUM_COST { get; set; }
        public string PREMIUM_COST_TYPE { get; set; }
        public string TYPE_OF_POLICY { get; set; }

        [Column("POLICY#")]
        public string POLICY_NUMBER { get; set; }

        public DateTime? CLAIMANT_DOCS { get; set; }
        public string CLAIM_NUMBER { get; set; }
        public decimal? AMOUNT_PAID { get; set; }
    }
}