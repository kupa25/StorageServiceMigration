using Newtonsoft.Json;

namespace Suddath.Helix.JobMgmt.Models
{
    public class EmailDto
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Value { get; set; }
        public int? TransfereeId { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string EmailType { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}