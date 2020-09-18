using Newtonsoft.Json;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models
{
    public class TransfereeDto
    {
        public int? Id { get; set; }

        [JsonProperty(PropertyName = "transfereeTitle")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "transfereeFirstName")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [JsonProperty(PropertyName = "transfereeLastName")]
        public string LastName { get; set; }

        public bool IsVip { get; set; }
        public IEnumerable<EmailDto> Emails { get; set; }
        public IEnumerable<PhoneDto> OriginPhones { get; set; }
        public IEnumerable<PhoneDto> DestinationPhones { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}