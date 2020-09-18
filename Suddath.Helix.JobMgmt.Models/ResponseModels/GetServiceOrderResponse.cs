using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetServiceOrderResponse
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAbbreviation { get; set; }
    }
}