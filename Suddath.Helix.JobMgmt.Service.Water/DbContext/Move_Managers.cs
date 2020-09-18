using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("MOVE_MANAGERS_INFO")]
    public class Move_Managers
    {
        public string USERNAME { get; set; }
        public string REAL_NAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
    }
}
