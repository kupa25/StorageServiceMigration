﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrderClaimInspection
    {
        public int Id { get; set; }
        public int ServiceOrderClaimId { get; set; }
        public DateTime? InspectionDate { get; set; }
        public decimal? Cost { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ServiceOrderInsuranceClaim ServiceOrderClaim { get; set; }
    }
}