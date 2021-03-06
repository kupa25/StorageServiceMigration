﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class JobSurveyInfo
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
        public DateTime? RequestedPackStartDateTime { get; set; }
        public DateTime? RequestedPackEndDateTime { get; set; }
        public string CriticalQualityNote { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? SendToPricingDate { get; set; }
        public DateTime? QuoteApprovedDate { get; set; }
        public DateTime? ActualSurveyDate { get; set; }
        public DateTime? QuoteSendDate { get; set; }

        public virtual Job Job { get; set; }
        public virtual Vendor SurveyVendor { get; set; }
    }
}