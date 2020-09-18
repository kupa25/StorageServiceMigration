using System;
using System.Collections.Generic;
using System.Text;
using Suddath.Helix.JobMgmt.Models.RequestModels.PaymentRequest;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class ServiceItemsPaymentUpdateRequest
    {
        public IEnumerable<ServiceItemPaymentUpdateRequest> ServiceItems { get; set; }
    }
}
