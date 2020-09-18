using System;
using System.Collections.Generic;
using System.Text;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;

namespace Suddath.Helix.JobMgmt.Models
{
    public class MoveServiceDetailDto
    {
        public string AgentName { get; set; }

        public AddressDto Address { get; set; }
        public AddressDto AgentAddress { get; set; }

        public DateTime? PackDate { get; set; }
        public DateTime? LoadDate { get; set; }

        public DateTime? BookDate { get; set; }

        public DateTime? SurveyDate { get; set; }

        public DateTime? OrignSITinDate { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public string ArriveDate { get; set; }
        public string ContactPhone { get; set; }
        public bool IsInsuranceRequired { get; set; }

        public ICollection<TrackerDto> TrackingDetails { get; set; }

    }
}
