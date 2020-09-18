using System;
using System.Collections.Generic;
using System.Text;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;

namespace Suddath.Helix.JobMgmt.Models
{
    public class ContactDto
    {
        public ContactDto()
        {
            Emails = new List<EmailDto>();
        }
        public string ContactType { get; set; }
        public string IdentityId { get; set; }
        public string Salutation { get;  set; }
        public string FirstName { get;  set; }
        public string MiddleName { get;  set; }
        public string LastName { get;  set; }
        public string Suffix { get;  set; }
        public byte[] Passport { get; set; }
        public byte[] TaxIdentifier { get; set; }
        public string Citizenship { get; set; }
        public IList<AddressDto> Addresses { get;  set; }
        public IList<PhoneDto> Phones { get;  set; }
        public IList<EmailDto> Emails { get;  set; }

    }
}
