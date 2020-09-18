using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;

namespace Suddath.Helix.JobMgmt.Models
{
    public class JobInfoDto
    {
        [JsonProperty(PropertyName = "origAddress")]
        public string OriginAddressLabel { get; set; }

        [JsonProperty(PropertyName = "additionalOrigAddr")]
        public string OriginAddressAdditionalInfo { get; set; }

        [JsonProperty(PropertyName = "destAddress")]
        public string DestinationAddressLabel { get; set; }

        [JsonProperty(PropertyName = "additionalDestAddr")]
        public string DestinationAddressAdditionalInfo { get; set; }

        public IEnumerable<AddressDto> Addresses { get; set; }
    }
}