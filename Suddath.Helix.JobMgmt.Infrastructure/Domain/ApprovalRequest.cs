﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ApprovalRequest
    {
        public ApprovalRequest()
        {
            ServiceItem = new HashSet<ServiceItem>();
        }

        public int Id { get; set; }
        public string ApprovalRequestStatus { get; set; }
        public DateTime? SentDateTime { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<ServiceItem> ServiceItem { get; set; }
    }
}