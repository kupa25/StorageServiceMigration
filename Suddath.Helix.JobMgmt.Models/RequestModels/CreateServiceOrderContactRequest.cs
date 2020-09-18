using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateServiceOrderContactRequest
    {
        public string FullName { get; set; }
        public string PhoneType { get; set; }
        public string PhoneCountryCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExtension { get; set; }
        public string Email { get; set; }
    }
}
