﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrderThirdPartyService
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public string ThirdPartyServiceName { get; set; }
        public string OriginOrDestination { get; set; }
        public decimal? Cost { get; set; }
        public string CostType { get; set; }
        public int? Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ServiceOrder ServiceOrder { get; set; }
    }
}