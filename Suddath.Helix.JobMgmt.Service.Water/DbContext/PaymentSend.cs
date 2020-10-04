using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("PAYMENTS_SENT")]
    public partial class PaymentSent
    {
        [Key]
        public string MOVES_ID { get; set; }

        [Key]
        public string NAMES_ID { get; set; }

        public string ACCOUNT_CODE { get; set; }
        public string ACCOUNT_DESCRIPTION { get; set; }
        public decimal? ESTIMATED_AMOUNT { get; set; }
        public decimal? AMOUNT { get; set; }
        public DateTime? DATE_BILLED { get; set; }
        public DateTime? DATE_PAID { get; set; }

        [NotMapped]
        public int? VendorID { get; set; }

        [NotMapped]
        public string BillToLable { get; set; }

        [Column("CHECK#")]
        public string CHECK { get; set; }

        [Column("INVOICE#")]
        public string INVOICE_NUMBER { get; set; }

        public DateTime? ACTUAL_POSTED { get; set; }
        public DateTime? ACCTG_DATE { get; set; }
        public decimal? ADJ_EST_AMOUNT { get; set; }
    }
}