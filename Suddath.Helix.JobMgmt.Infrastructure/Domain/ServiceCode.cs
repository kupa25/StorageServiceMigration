﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceCode
    {
        public ServiceCode()
        {
            ServiceItem = new HashSet<ServiceItem>();
        }

        public string ServiceCode1 { get; set; }
        public string ServiceCodeDescription { get; set; }
        public string ActivityTypeCode { get; set; }
        public bool? IsActive { get; set; }

        public virtual ActivityType ActivityTypeCodeNavigation { get; set; }
        public virtual ICollection<ServiceItem> ServiceItem { get; set; }
    }
}