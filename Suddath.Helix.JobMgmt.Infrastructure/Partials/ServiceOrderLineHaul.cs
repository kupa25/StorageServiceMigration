using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrderLineHaul
    {
        public ServiceOrderLineHaul()
        {
            LineHaulType = "Drop and Pick";
            IsChassisProvided = false;
            Currency = "USD";
        }
    }
}