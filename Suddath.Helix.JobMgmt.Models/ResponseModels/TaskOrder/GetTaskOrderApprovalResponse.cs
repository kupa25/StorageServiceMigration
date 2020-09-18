using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderApprovalResponse
    {
        //Service Item Id
        public int JobId { get; set; }

        public ICollection<GetTaskOrderCrateApprovalResponse> Crates { get; set; }
        public ICollection<GetTaskOrderShuttleApprovalResponse> Shuttles { get; set; }
        public ICollection<GetTaskOrderStorageApprovalResponse> Storages { get; set; }
        public ICollection<GetTaskOrderServiceItemApprovalResponse> ServiceItems { get; set; }
    }
}