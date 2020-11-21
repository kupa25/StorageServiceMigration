using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class JobDto
    {
        public int JobId { get; set; }

        [Required]
        public string BillToLabel { get; set; }

        [Required]
        public string BillToType { get; set; }

        [Required]
        public NameIdDto BillTo { get; set; }

        [Required]
        public string AccountLabel { get; set; }

        [Required]
        public NameIdDto Account { get; set; }

        public string BookerLabel { get; set; }
        public NameIdDto Booker { get; set; }
        public string AuthPoNum { get; set; }

        [Required]
        public string MoveType { get; set; }

        [Required]
        public string RevenueType { get; set; }

        [Required]
        public string BranchName { get; set; }

        public string BranchDisplayName { get; set; }
        public string BranchCode { get; set; }
        public string AccountCustomerReference { get; set; }

        public string Status { get; set; }
        public bool IsSurveyAndBid { get; set; }
        public string AccrualStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public string ExternalReference { get; set; }
        public string ExternalReferenceDescription { get; set; }

        [JsonIgnore]
        public string JobSource { get; set; }
    }
}