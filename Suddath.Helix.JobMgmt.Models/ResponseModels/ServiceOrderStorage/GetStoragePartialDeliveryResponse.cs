using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage
{
    public class GetStoragePartialDeliveryResponse
    {
        public int Id { get; set; }
        public int? NetWeightLb { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public int? TotalWeightLb { get; set; }
        public string User { get; set; }
        public bool IsEditable { get; set; }
        public string Module { get; set; }
    }
}