﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class Service
    {
        public Service()
        {
            InverseSuperService = new HashSet<Service>();
            ServiceOrder = new HashSet<ServiceOrder>();
        }

        public int Id { get; set; }
        public int SuperServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAbbreviation { get; set; }
        public int SortOrder { get; set; }

        public virtual Service SuperService { get; set; }
        public virtual ICollection<Service> InverseSuperService { get; set; }
        public virtual ICollection<ServiceOrder> ServiceOrder { get; set; }
    }
}