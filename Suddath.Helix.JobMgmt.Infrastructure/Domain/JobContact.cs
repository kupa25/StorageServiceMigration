﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class JobContact
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string ContactType { get; set; }
        public string FullName { get; set; }
        public string PhoneCountryCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExtension { get; set; }
        public string Email { get; set; }

        public virtual Job Job { get; set; }
    }
}