using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class ServiceDto 
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public IList<ServiceDto> ChildServices { get; set; }
    }
}
