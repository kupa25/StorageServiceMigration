﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class InvoiceStatus
    {
        public InvoiceStatus()
        {
            Invoice = new HashSet<Invoice>();
        }

        public string InvoiceStatusIdentifier { get; set; }
        public string InvoiceStatusName { get; set; }
        public string InvoiceStatusDescription { get; set; }

        public virtual ICollection<Invoice> Invoice { get; set; }
    }
}