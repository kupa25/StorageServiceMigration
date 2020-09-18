using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("NAMES")]
    public partial class Name
    {
        [Key]
        [Column("NAMES_ID", Order = 0)]
        public string Id { get; set; }

        [Column("FIRST_NAME")]
        public string FirstName { get; set; }

        [Column("LAST_NAME")]
        public string LastName { get; set; }

        [Column("VIP")]
        public string IsVip { get; set; }

        //   [ForeignKey("Move")]
        [Column("GROUP_CODE")]
        public string ShipperMoveId { get; set; }

        [Column("ORIGIN_PICKUP")]
        public string ORIGIN_PICKUP { get; set; }

        [Column("PHONE1")]
        public string PHONE1 { get; set; }

        [Column("PHONE2")]
        public string PHONE2 { get; set; }

        [Column("E_MAIL")]
        public string Email1 { get; set; }

        [Column("E_MAIL2")]
        public string Email2 { get; set; }

        [Column("E_MAIL3")]
        public string Email3 { get; set; }

        [Column("STREET")]
        public string Street { get; set; }

        [Column("APT")]
        public string Appartment { get; set; }

        [Column("CITY")]
        public string City { get; set; }

        [Column("STATE")]
        public string State { get; set; }

        [Column("ZIP")]
        public string Zip { get; set; }

        [Column("COUNTRY")]
        public string Country { get; set; }

        public virtual Move Move { get; set; }

        [Column("TITLE")]
        public string Title { get; set; }

        [Column("EPASSPORT")]
        public byte[] Passport { get; set; }

        [Column("ESSN")]
        public byte[] TaxIdentifier { get; set; }

        [Column("PASSPORT_COUNTRY")]
        public string Passport_Country { get; set; }

        [Column("DATE_OF_A_OR_D")]
        public DateTime? DateOfOriginOrDestination { get; set; }
    }
}