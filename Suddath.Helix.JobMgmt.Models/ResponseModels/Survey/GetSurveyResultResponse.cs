using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.Survey
{
    public class GetSurveyResultResponse
    {
        public int SuperServiceOrderId { get; set; }
        public int SuperServiceId { get; set; }
        public string SuperServiceName { get; set; }
        public string SuperServiceModeName { get; set; }
        public string SuperServiceIconName { get; set; }
        public decimal? GrossWeightLb { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightLb { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? GrossVolumeCUFT { get; set; }
        public decimal? GrossVolumeCBM { get; set; }
        public decimal? NetVolumeCUFT { get; set; }
        public decimal? NetVolumeCBM { get; set; }
        public decimal? ACWLb { get; set; }
        public decimal? ACWKg { get; set; }
        public decimal? TotalCrateVolumeCUFT { get; set; }
        public decimal? TotalCrateVolumeCBM { get; set; }
        public IEnumerable<GetSurveyResultAccessorialResponse> Accessorials { get; set; }
        public IEnumerable<GetSurveyResultThirdPartyServiceResponse> ThirdParyServices { get; set; }
        public IEnumerable<GetSurveyResultCrateResponse> Crates { get; set; }
    }

    public class GetSurveyResultAccessorialResponse
    {
        public int Id { get; set; }
        public int SuperServiceOrderId { get; set; }
        public string AccessorialName { get; set; }
        public int Count { get; set; }
    }

    public class GetSurveyResultThirdPartyServiceResponse
    {
        public int Id { get; set; }
        public int SuperServiceOrderId { get; set; }
        public string ThirdPartyServiceName { get; set; }
        public int Count { get; set; }
    }

    public class GetSurveyResultCrateResponse
    {
        public int Id { get; set; }
        public int SuperServiceOrderId { get; set; }
        public string Description { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Depth { get; set; }
        public bool IsMetric { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
    }
}