using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class GetSuperServicesResponse
    {
        public int Id { get; set; }
        public string SuperServiceName { get; set; }
        public List<GetSuperServiceModeResponse> SuperServiceMode { get; set; }
    }

    public class GetSuperServiceModeResponse
    {
        public int Id { get; set; }
        public string ModeName { get; set; }
    }
}