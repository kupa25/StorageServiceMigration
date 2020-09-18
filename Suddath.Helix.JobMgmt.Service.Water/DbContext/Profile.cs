using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("PROFILES")]
    public partial class Profile
    {
        [Key]
        [ForeignKey("Moves")]
        [Column("NAMES_ID")]
        public string NamesId { get; set; }

        [Column("B_INSURANCE_AUTH")]
        public string InsuranceAuth { get; set; }

        [Column("DATE_ENTERED")]
        public DateTime DateEntered { get; set; }
    }

    public partial class Profile
    {
        public bool IsInsuranceRequired
        {
            get
            {
                return String.Equals(InsuranceAuth?.ToUpper(), "Y");
            }
        }
    }
}
