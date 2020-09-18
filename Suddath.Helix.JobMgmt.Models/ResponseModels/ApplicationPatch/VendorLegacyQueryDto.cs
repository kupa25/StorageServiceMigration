using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ApplicationPatch
{
    public class VendorLegacyQueryDto
    {
        public int Id { get; set; }

        [JsonProperty("Value")]
        public string AccountingId { get; set; }

        public string Name { get; set; }

        [JsonProperty("Description")]
        public string ShortAddress { get; set; }
    }
}