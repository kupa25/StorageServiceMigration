using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderStorageApprovalResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Status { get; set; }
        public string ShipmentNumber { get; set; }
        public string ServiceCode { get; set; }
        public DateTime? SitInDate { get; set; }
        public DateTime? FirstAvailableDate { get; set; }
        public decimal? SitWeight { get; set; }
        public int? SitDaysRequested { get; set; }
        public string Remarks { get; set; }
    }
}
