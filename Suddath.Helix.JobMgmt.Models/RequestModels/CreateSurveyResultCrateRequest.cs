using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateSurveyResultCrateRequest
    {
        public string Description { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Depth { get; set; }
        public bool IsMetric { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
    }
}