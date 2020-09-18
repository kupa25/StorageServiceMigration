﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class Entitlement
    {
        public int JobId { get; set; }
        public int AuthorizedWeightLb { get; set; }
        public int ProGearWeightLb { get; set; }
        public int ProGearWeightSpouseLb { get; set; }
        public int TotalWeightLb { get; set; }
        public bool IsDependentsAuthorized { get; set; }
        public bool IsNonTemporaryStorage { get; set; }
        public bool IsPrivatelyOwnedVehicle { get; set; }
        public int StorageInTransitDays { get; set; }
        public int TotalDependents { get; set; }
        public DateTime? EffectiveStartDateTime { get; set; }
        public DateTime? EffectiveEndDateTime { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual TaskOrder Job { get; set; }
    }
}