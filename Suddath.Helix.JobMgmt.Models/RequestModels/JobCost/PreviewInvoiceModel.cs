using Suddath.Helix.Common.Infrastructure.EventBus.Events;

namespace Suddath.Helix.JobMgmt.Models.RequestModels.JobCost
{
    public class PreviewInvoiceModel : CreateInvoiceRequest
    {
        public string AccountingId { get; set; }
        public string InvoiceNumber { get; set; }
        public AddressDto BillToAddress { get; set; }
    }
}
