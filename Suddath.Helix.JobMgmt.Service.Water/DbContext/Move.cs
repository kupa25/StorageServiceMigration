using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("MOVES")]
    public partial class Move
    {
        public Move()
        {
            MoveItems = new HashSet<MoveItem>();
            MoveAgents = new HashSet<MoveAgent>();
            Names = new HashSet<Name>();
        }

        [Key]
        [Column("MOVES_ID")]
        public string Id { get; set; }

        [Column("NAMES_ID")]
        public string VendorId { get; set; }

        [Column("DATE_ENTERED")]
        public DateTime? DateEntered { get; set; } //BookDate

        [Column("DATE_LAST_CHANGE")]
        public DateTime DateLastChanged { get; set; }

        [Column("ORIGIN")]
        public string Origin { get; set; }

        [Column("DESTINATION")]
        public string Destination { get; set; }

        [Column("REG#")]
        public string RegNumber { get; set; }

        [Column("BOOKER")]
        public string Booker { get; set; }

        public string BILL { get; set; }

        [Column("GROUP_CODE")]
        public string GroupCode { get; set; }

        public string SHIPMENT_TYPE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string MODE_OF_TRANSPORT { get; set; }
        public string SERVICE { get; set; }

        public string TYPE_OF_MOVE { get; set; }
        public string MOVE_MANAGER { get; set; }

        [NotMapped]
        public string MOVE_MANAGER_EMAIL { get; set; }

        [NotMapped]
        public string MOVE_MANAGER_PHONE { get; set; }

        public string TRAFFIC_MANAGER { get; set; }
        public int? AUTHORIZED_WEIGHT { get; set; }

        [Column("CORPORATE")]
        public string AccountId { get; set; }

        public virtual Name Account { get; set; }

        public int? REWEIGH { get; set; }
        public string ORIGIN_TO_ID { get; set; }
        public string DESTINATION_TO_ID { get; set; }

        [Column("PO#")]
        public string PONumber { get; set; }

        public virtual ICollection<Name> Names { get; set; }

        public virtual ICollection<MoveAgent> MoveAgents { get; set; }
        public ICollection<MoveItem> MoveItems { get; set; }

        [NotMapped]
        public ICollection<MoveTracking> MoveTrackings { get; set; } //Can not use this in an include; its a view

        public virtual Profile Profile { get; set; }
    }
}