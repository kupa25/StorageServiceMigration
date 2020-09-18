using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderThirdParty
{
    public class GetThirdPartyCrateResponse
    {
        public int Id { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string OriginOrDestination { get; set; }
        public string Description { get; set; }
        public string CrateType { get; set; }
        public int? Quantity { get; set; }
        public bool IsMetric { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Volume { get; set; }
    }
}