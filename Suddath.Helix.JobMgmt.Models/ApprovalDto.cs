using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class ApprovalDto
    {
        public string ServiceItemIdentifier { get; set; }
        public string SourceSystem { get; set; }
        public string RequestedServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string Description { get; set; }
        public string CrateDescription { get; set; }
        public string PickupPostalCode { get; set; }
        public string Address { get; set; }
        public string ShipmentNumber { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public DateTime? TimeMilitary1 { get; set; }
        public DateTime? FirstAvailableDeliveryDateTime1 { get; set; }
        public DateTime? TimeMilitary2 { get; set; }
        public DateTime? FirstAvailableDeliveryDateTime2 { get; set; }
        public DateTime? SitInDate { get; set; }
        public decimal? SitWeight { get; set; }
        public int? SitDaysRequested { get; set; }
        public DateTime? FirstAvailableDate { get; set; }
    }
}