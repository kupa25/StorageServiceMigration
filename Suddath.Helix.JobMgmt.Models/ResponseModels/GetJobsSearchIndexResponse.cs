using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class GetJobsSearchIndexResponse
    {
        public int JobId { get; set; }
        public string TransfereeFirstName { get; set; }
        public string TransfereeLastName { get; set; }
        public string MasterBolNumber { get; set; }
        public string MasterAwbNumber { get; set; }
        public ICollection<string> ContainerNumbers { get; set; }
        public ICollection<JobSearchInvoiceDocument> InvoiceDocuments { get; set; }
    }

    public class JobSearchInvoiceDocument
    {
        public int SuperServiceOrderId { get; set; }
        public string SuperServiceName { get; set; }
        public int ItemId { get; set; }
        public string DisplayId { get; set; }
        public string InvoiceNumber { get; set; }
        public string VendorName { get; set; }
        public string VendorAccountingId { get; set; }
        public string VendorShortAddress { get; set; }
    }
}