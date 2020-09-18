using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderCrateApprovalResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Status { get; set; }
        public string ShipmentNumber { get; set; }
        public string ServiceCode { get; set; }
        public string CrateDescription { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
    }
}