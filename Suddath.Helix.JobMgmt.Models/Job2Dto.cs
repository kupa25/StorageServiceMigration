using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class Job2Dto
    {
        [JsonProperty(PropertyName = "Move")]
        public JobDto Job { get; set; }

        [JsonProperty(PropertyName = "MoveInfo")]
        public JobInfoDto JobInfo { get; set; }
        public TransfereeDto Transferee { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}