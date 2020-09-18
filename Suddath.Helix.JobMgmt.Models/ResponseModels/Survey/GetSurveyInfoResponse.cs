using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.Survey
{
    public class GetSurveyInfoResponse
    {
        public int JobId { get; set; }
        public int? SurveyVendorId { get; set; }
        public string SurveyVendorName { get; set; }
        public string SurveyorName { get; set; }
        public string OriginAgentName { get; set; }
        public string DestinationAgentName { get; set; }
        public DateTimeOffset? RequestedSurveyDateTime { get; set; }
        public DateTimeOffset? ScheduledSurveyDateTime { get; set; }
        public DateTime? RequestedPackStartDate { get; set; }
        public DateTime? RequestedPackEndDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? SendToPricingDate { get; set; }
        public DateTime? QuoteApprovedDate { get; set; }
        public DateTime? ActualSurveyDate { get; set; }
        public DateTime? QuoteSendDate { get; set; }
        public string CriticalQualityNote { get; set; }
    }
}