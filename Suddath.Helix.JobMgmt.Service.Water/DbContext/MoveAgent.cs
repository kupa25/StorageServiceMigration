using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("MOVE_AGENTS")]
    public class MoveAgent
    {
        [Key, ForeignKey("Move")]
        [Column("MOVES_ID", Order = 0)]
        public string Id { get; set; }

        [Key, ForeignKey("Name")]
        [Column("NAMES_ID", Order = 1)]
        public string VendorNameId { get; set; }

        public DateTime? DOCS_RCV_DATE { get; set; }

        [Key]
        [Column("JOB_CATEGORY", Order = 2)]
        public string JobCategory { get; set; } //ORIGIN, DESTINATION or INLAND

        [Column("ACT_PK_DATE1")]
        public DateTime? PackDate { get; set; }

        [Column("ACT_PU_DATE1")]
        public DateTime? PickupDate { get; set; }

        [Column("REQ_DELIVERY_DATE1")]
        public DateTime? RequiredDeliveryDate { get; set; }

        [Column("ACT_DELIVERY_DATE1")]
        public DateTime? ActualDeliveryDate { get; set; }

        public DateTime? UNPACK_DATE1 { get; set; }

        [Column("EST_AR_DATE1")]
        public DateTime? EstArrivalDate1 { get; set; }

        [Column("EST_AR_DATE2")]
        public DateTime? EstArrivalDate2 { get; set; }

        public DateTime? ACT_DEPARTURE_DATE { get; set; }
        public DateTime? EST_DEPARTURE_DATE { get; set; }
        public DateTime? ACT_AR_DATE1 { get; set; }
        public DateTime? ACT_AR_DATE2 { get; set; }
        public DateTime? EST_PU_DATE1 { get; set; }
        public DateTime? EST_PU_DATE2 { get; set; }

        public DateTime? ACT_SURVEY_DATE { get; set; }

        public DateTime? EST_SURVEY_DATE { get; set; }

        [Column("CUSTOM_DATE1")]
        public DateTime? CustomsInDate { get; set; }

        [Column("CUSTOM_DATE2")]
        public DateTime? CustomsOutDate { get; set; }

        [Column("SIT_IN")]
        public DateTime? SITinDate { get; set; }

        [Column("SIT_OUT")]
        public DateTime? SIToutDate { get; set; }

        [Column("SURVEY_WEIGHT")]
        public int? SurveyWeight { get; set; }

        [Column("PORT_LD")]
        public string PortLoad { get; set; }

        [Column("PORT_DC")]
        public string PortDischarge { get; set; }

        [MaxLength(40)]
        public string CONTAINER_SIZES { get; set; }

        [MaxLength(40)]
        public string VSL_VOY1 { get; set; }

        [MaxLength(40)]
        public string VSL_VOY2 { get; set; }

        public virtual Move Move { get; set; }
        public virtual Name Name { get; set; }
    }
}