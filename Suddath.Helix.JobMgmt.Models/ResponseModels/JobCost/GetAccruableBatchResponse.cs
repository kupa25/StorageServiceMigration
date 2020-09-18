using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetAccruableBatchResponse
    {
        public int SuperServiceOrderId { get; set; }
        public string SuperServiceOrderDisplayId { get; set; }
        public int JobId { get; set; }
        public int JobCostServiceOrderId { get; set; }
        public DateTime AccrualPendingDateTime { get; set; }
        public DateTime? ActualPackEndDate { get; set; }
        public string BranchName { get; set; }
        public string RevenueType { get; set; }
        public string MoveConsultantName { get; set; }
        public string AccountEntityName { get; set; }
        public string MoveType { get; set; }
        public decimal? NetWeightLb { get; set; }
        public decimal? GrossWeightLb { get; set; }
        public decimal? TotalAccruableRevenue { get; set; }
        public decimal? TotalAccruableProfit { get; set; }
        public decimal? MarginPercentage { get; set; }
    }
}