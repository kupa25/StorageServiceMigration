using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("NOTES")]
    public partial class Notes
    {
        public string TABLE_NAME { get; set; }
        public string TABLE_ID { get; set; }
        public string NOTE { get; set; }
        public DateTime? CALLBACK_DATE { get; set; }
        public string ENTERED_BY { get; set; }
        public DateTime DATE_ENTERED { get; set; }
        public string CATEGORY { get; set; }

        [Key]
        public int NOTES_SEQ { get; set; }
    }
}