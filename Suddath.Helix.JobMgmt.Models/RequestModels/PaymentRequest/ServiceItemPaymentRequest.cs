namespace Suddath.Helix.JobMgmt.Models.RequestModels
{
    public class ServiceItemPaymentRequest
    {
        public int ServiceItemId { get; set; }
        public decimal RequestedPriceUSD { get; set; }
    }
}
