using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class MoveDto
    {
        public int MoveId { get; set; }
        public string BillToLabel { get; set; }

        [JsonIgnore]
        public string BillToType { get; set; }

        public NameIdDto BillTo { get; set; }
        public string AccountLabel { get; set; }
        public NameIdDto Account { get; set; }
        public string BookerLabel { get; set; }
        public NameIdDto Booker { get; set; }
        public string AuthPoNum { get; set; }
        public string MoveType { get; set; }
        public string RevenueType { get; set; }
        public string BranchName { get; set; }
    }
}