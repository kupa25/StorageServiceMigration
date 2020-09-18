using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models
{
    public class PhoneDto
    {
        [JsonIgnore]
        public int Id { get; set; }

        public bool? Primary { get; set; }
        public string PhoneType { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string NationalNumber { get; set; } //Must store as E.164 Format

        [JsonProperty(PropertyName = "ext")]
        public string Extension { get; set; }

        [JsonProperty(PropertyName = "locationType")]
        public string LocationType { get; set; }

        public string DialCode { get; set; }
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}