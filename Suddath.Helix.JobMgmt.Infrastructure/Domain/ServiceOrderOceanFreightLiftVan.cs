﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrderOceanFreightLiftVan
    {
        public int Id { get; set; }
        public int ServiceOrderOceanFreightContainerId { get; set; }
        public int? NumberOfItems { get; set; }
        public string Description { get; set; }
        public decimal? NetWeightLb { get; set; }
        public decimal? GrossWeightLb { get; set; }
        public decimal? VolumeCUFT { get; set; }
        public string DimensionsFT { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ServiceOrderOceanFreightContainer ServiceOrderOceanFreightContainer { get; set; }
    }
}