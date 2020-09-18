using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class ServiceItemsPaymentRequest
    {
        public IEnumerable<ServiceItemPaymentRequest> ServiceItems { get; set; }
    }
}
