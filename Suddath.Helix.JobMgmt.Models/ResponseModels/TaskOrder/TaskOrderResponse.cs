using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class TaskOrderResponse : TaskOrderDto
    {
        public TaskOrderResponse()
        {
            Alerts = new HashSet<string>();
        }

        public int TaskOrderId { get; set; }

        public string CET { get; set; }

        public string OPS { get; set; }

        public string Status { get; set; }

        public string PreferredName { get; set; }

        public string PreferredContactMethod { get; set; }

        public string AuthorizedRepresentative { get; set; }

        public string MilitaryBranch { get; set; }

        public string Rank { get; set; }

        public string ServiceMemberName { get; set; }

        public string OriginAddress { get; set; }

        public string DestinationAddress { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<string> Alerts { get; set; }

        public ICollection<TaskOrderServiceResponse> Services { get; set; }
        public TaskOrderAddressInfoResponse AddressInfo { get; set; }
    }
}