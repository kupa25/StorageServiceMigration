using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("MOVE_ITEMS")]
    public class MoveItem
    {
        [Key, ForeignKey("Move")]
        [Column("MOVES_ID")]
        public string Id { get; set; }
        public string DESCRIPTION { get; set; }

        [Column("NET_WEIGHT")]
        public int? NetWeight { get; set; }

        [Column("WEIGHT")]
        public int? Weight { get; set; }

        [Column("NUMBER_OF_PIECES")]
        public int? NumberOfPieces { get; set; }

        public virtual Move Move { get; set; }

    }
}
