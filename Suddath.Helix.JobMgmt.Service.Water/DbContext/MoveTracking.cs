using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("MOVE_TRACKING_HELIX")]
    public class MoveTracking
    {
        [ForeignKey("Move")]
        [Column("MOVES_ID")]
        public string Id { get; set; }
        public virtual Move Move { get; set; }

        [Column("EVENT_DATE")]
        public DateTime? EventDate { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("SECTION")]
        public int? Section { get; set; }

    }
}
