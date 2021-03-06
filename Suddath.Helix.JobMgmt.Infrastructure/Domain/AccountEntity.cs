﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class AccountEntity
    {
        public AccountEntity()
        {
            BillableItem = new HashSet<BillableItem>();
            Invoice = new HashSet<Invoice>();
            Job = new HashSet<Job>();
            PayableItem = new HashSet<PayableItem>();
            ServiceOrderStorageRevenue = new HashSet<ServiceOrderStorageRevenue>();
            VendorInvoice = new HashSet<VendorInvoice>();
        }

        public int Id { get; set; }
        public string AccountingId { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string ShortAddress { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<BillableItem> BillableItem { get; set; }
        public virtual ICollection<Invoice> Invoice { get; set; }
        public virtual ICollection<Job> Job { get; set; }
        public virtual ICollection<PayableItem> PayableItem { get; set; }
        public virtual ICollection<ServiceOrderStorageRevenue> ServiceOrderStorageRevenue { get; set; }
        public virtual ICollection<VendorInvoice> VendorInvoice { get; set; }
    }
}