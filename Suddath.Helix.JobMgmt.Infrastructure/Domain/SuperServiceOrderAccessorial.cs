﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class SuperServiceOrderAccessorial
    {
        public int Id { get; set; }
        public int SuperServiceOrderId { get; set; }
        public string AccessorialName { get; set; }
        public string ThirdPartyServiceName { get; set; }
        public int? Count { get; set; }

        public virtual SuperServiceOrder SuperServiceOrder { get; set; }
    }
}