using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderOceanFreight
{
    public class GetOceanFreightContainerResponse
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public string ContainerType { get; set; }
        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
        public int? NumberOfItems { get; set; }
        public decimal? TareWeightLb { get; set; }
        public string Description { get; set; }
        public IEnumerable<GetOceanFreightContainerLooseItemResponse> LooseItems { get; set; }
        public IEnumerable<GetOceanFreightContainerLiftVanResponse> LiftVans { get; set; }
        public IEnumerable<GetOceanFreightContainerVehicleResponse> Vehicles { get; set; }
    }
}