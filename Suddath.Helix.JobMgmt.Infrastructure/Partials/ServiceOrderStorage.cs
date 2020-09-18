using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrderStorage
    {
        public ServiceOrderStorage()
        {
            StorageCurrency = "USD";
            InsuranceCurrency = "USD";
        }
    }
}