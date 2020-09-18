using System;
using System.Collections.Generic;
using System.Text;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;

namespace Suddath.Helix.JobMgmt.Models
{
    public class AccountEntityDTO
    {
        public int AccountEntityId { get; set; }
        public string TenantId { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Alias { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string Code { get; set; }
        public AddressDto Address { get; set; }
        public PhoneDto Phone { get; set; }
        public string AccountingId { get; set; }
        public string AccountManager { get; set; }
        public string CustomerServiceManager { get; set; }
        public string SalesPerson { get; set; }
        public string Booker { get; set; }
    }
}