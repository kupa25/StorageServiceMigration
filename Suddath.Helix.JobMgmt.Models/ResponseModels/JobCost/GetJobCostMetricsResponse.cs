using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class GetJobCostMetricsResponse
    {
        public decimal? GrossWeightLb { get; set; }
        public decimal? NetWeightLb { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? ACWWeightLb { get; set; }
        public decimal? GrossVolumeCUFT { get; set; }
        public decimal? GrossVolumeCBM { get; set; }
        public decimal? NetVolumeCUFT { get; set; }
        public decimal? NetVolumeCBM { get; set; }
        public decimal? SurveyNetWeightLb { get; set; }
        public decimal? SurveyGrossWeightLb { get; set; }
        public decimal? AuthorizedNetWeight { get; set; }
        public decimal? AuthorizedGrossWeight { get; set; }
        public decimal? OverweightPercentage { get; set; }
    }
}