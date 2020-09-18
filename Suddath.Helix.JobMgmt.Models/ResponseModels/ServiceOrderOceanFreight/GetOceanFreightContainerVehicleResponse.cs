using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderOceanFreight
{
    public class GetOceanFreightContainerVehicleResponse
    {
        public int Id { get; set; }
        public int OceanFreightContainerId { get; set; }
        public string VehicleType { get; set; }
        public string VIN { get; set; }
        public string TitleNumber { get; set; }
        public decimal? WeightLb { get; set; }
        public decimal? VolumeCUFT { get; set; }
        public decimal? VehicleValue { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColor { get; set; }
        public DateTime? TitleFromShipperReceivedDate { get; set; }
        public DateTime? DrainStatementReceivedDate { get; set; }
        public DateTime? TitleToShipperSentDate { get; set; }
    }
}