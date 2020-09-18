using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class CreateServiceItemsPaymentRequest
    {
        public IEnumerable<CreateServiceItemPaymentRequest> ServiceItems { get; set; }
    }

    public class CreateServiceItemPaymentRequest
    {
        public int ServiceItemId { get; set; }
        public decimal RequestedPriceUSD { get; set; }
    }
}