﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrderStoragePartialDelivery
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public decimal? NetWeightLb { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public int? ReferenceId { get; set; }
        public string ReferenceType { get; set; }
        public string UserName { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ServiceOrder ServiceOrder { get; set; }
    }
}