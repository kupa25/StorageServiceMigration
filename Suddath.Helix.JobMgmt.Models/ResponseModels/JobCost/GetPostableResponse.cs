using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost
{
    public class PostableBase
    {
        public string RecordType { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string DocNum { get; set; }
        public string DocType { get; set; }
        public string GPDatabaseName => "SI";
        public string PostingUserName { get; set; }
        public int? ImportStatus { get; set; }
        public string ImportDescription { get; set; }
        public string RegNumber { get; set; }

        [Description("TransfereeLastName, TransfereeFirstName")]
        public string ShipperName { get; set; }
    }

    public class PostableHeader : PostableBase
    {
        public string Desc { get; set; }
        public decimal DocAmt { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime PostingDate { get; set; }
    }

    public class AccruableARHeader : PostableHeader
    {
        public int BillableItemId { get; set; }
        public int InvoiceId { get; set; }
        public string ActualCustomerId { get; set; }
    }

    public class AccruableAPHeader : PostableHeader
    {
        public int PayableItemId { get; set; }
        public int VendorInvoiceId { get; set; }
        public string ActualVendorId { get; set; }
    }

    public class PostableLineItem : PostableBase
    {
        public int? BillableItemId { get; set; }
        public int? PayableItemId { get; set; }
        public decimal DistAmt { get; set; }
        public int? DistType { get; set; }

        [Description("superServiceOrderDisplayId")]
        public string MovesId { get; set; }

        public int Sequence { get; set; }
        public string ItemCode { get; set; }
        public string AcctNum { get; set; }
    }

    public class GetPostableResponse
    {
        public int SuperServiceOrderId { get; set; }
        public string DisplayId { get; set; }
        public DateTime? ActualPackEndDate { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public PostableHeader Header { get; set; }
        public List<PostableLineItem> LineItems { get; set; }
    }
}