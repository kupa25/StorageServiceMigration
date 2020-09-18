using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderOceanFreight
{
    public class GetOceanFreightContainerLiftVanResponse
    {
        public int Id { get; set; }
        public int OceanFreightContainerId { get; set; }
        public int? NumberOfItems { get; set; }
        public string Description { get; set; }
        public decimal? NetWeightLb { get; set; }
        public decimal? GrossWeightLb { get; set; }
        public decimal? VolumeCUFT { get; set; }
        public string DimensionsFT { get; set; }
    }
}