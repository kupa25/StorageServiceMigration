using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    [Table("V_MOVEINFORMATION")]
    public partial class MoveInformation
    {
        [Key]
        [Column("MOVES_ID", Order = 0)]
        public string MOVES_ID { get; set; }
        [Column("SYSTEM")]
        public string SYSTEM { get; set; }
        [Column("GBL")]
        public string GBL { get; set; }
        [Column("MODE_OF_TRANSPORT")]
        public string MODE_OF_TRANSPORT { get; set; }
        [Column("SCAC")]
        public string SCAC { get; set; }
        [Column("STATUS")]
        public string STATUS { get; set; }
        [Column("SHIPMENT_TYPE_ID")]
        public int SHIPMENT_TYPE_ID { get; set; }
        [Column("SHIPMENT_TYPE")]
        public string SHIPMENT_TYPE { get; set; }
        [Column("RELOCATION_TYPE_ID")]
        public int RELOCATION_TYPE_ID { get; set; }
        [Column("CUSTOMER_FIRST_NAME")]
        public string CUSTOMER_FIRST_NAME { get; set; }
        [Column("CUSTOMER_LAST_NAME")]
        public string CUSTOMER_LAST_NAME { get; set; }
        [Column("VIP")]
        public string VIP { get; set; }
        [Column("REQUIRED_DELIVERY_DATE")]
        public DateTime? REQUIRED_DELIVERY_DATE { get; set; }
        [Column("BOOKED_DATE")]
        public DateTime? BOOKED_DATE { get; set; }
        [Column("PACK_DATE")]
        public DateTime? PACK_DATE { get; set; }
        [Column("PREFERRED_DELIVERY_DATE")]
        public DateTime? PREFERRED_DELIVERY_DATE { get; set; }
        [Column("ACTUAL_DELIVERY_DATE")]
        public DateTime? ACTUAL_DELIVERY_DATE { get; set; }
        [Column("SIT_IN_DATE")]
        public DateTime? SIT_IN_DATE { get; set; }
        [Column("PICKUP_DATE")]
        public DateTime? PICKUP_DATE { get; set; }
        [Column("TRANSPORTATION_COORDINATOR")]
        public string TRANSPORTATION_COORDINATOR { get; set; }
        [Column("LOGISTIC_COORDINATOR")]
        public string LOGISTIC_COORDINATOR { get; set; }
        [Column("ORIGIN_ADDRESS")]
        public string ORIGIN_ADDRESS { get; set; }
        [Column("ORIGIN_CITY")]
        public string ORIGIN_CITY { get; set; }
        [Column("ORIGIN_STATE")]
        public string ORIGIN_STATE { get; set; }
        [Column("ORIGIN_COUNTRY")]
        public string ORIGIN_COUNTRY { get; set; }
        [Column("ORIGIN_POSTAL_CODE")]
        public string ORIGIN_POSTAL_CODE { get; set; }
        [Column("DESTINATION_ADDRESS")]
        public string DESTINATION_ADDRESS { get; set; }
        [Column("DESTINATION_CITY")]
        public string DESTINATION_CITY { get; set; }
        [Column("DESTINATION_STATE")]
        public string DESTINATION_STATE { get; set; }
        [Column("DESTINATION_COUNTRY")]
        public string DESTINATION_COUNTRY { get; set; }
        [Column("DESTINATION_POSTAL_CODE")]
        public string DESTINATION_POSTAL_CODE { get; set; }

        [Column("ORIGIN_EMAIL")]
        public string ORIGIN_EMAIL { get; set; }
        [Column("ORIGIN_EMAIL2")]
        public string ORIGIN_EMAIL2 { get; set; }
        [Column("ORIGIN_EMAIL3")]
        public string ORIGIN_EMAIL3 { get; set; }
        [Column("ORIGIN_EMAIL4")]
        public string ORIGIN_EMAIL4 { get; set; }
        [Column("ORIGIN_ShipperCell")]
        public string ORIGIN_ShipperCell { get; set; }
        [Column("ORIGIN_SpouseCell")]
        public string ORIGIN_SpouseCell { get; set; }
        [Column("ORIGIN_AgentCell")]
        public string ORIGIN_AgentCell { get; set; }
        [Column("ORIGIN_AlternateCell")]
        public string ORIGIN_AlternateCell { get; set; }
        [Column("DEST_EMAIL")]
        public string DEST_EMAIL { get; set; }
        [Column("DEST_EMAIL2")]
        public string DEST_EMAIL2 { get; set; }
        [Column("DEST_EMAIL3")]
        public string DEST_EMAIL3 { get; set; }
        [Column("DEST_EMAIL4")]
        public string DEST_EMAIL4 { get; set; }
        [Column("DEST_ShipperCell")]
        public string DEST_ShipperCell { get; set; }
        [Column("DEST_SpouseCell")]
        public string DEST_SpouseCell { get; set; }
        [Column("DEST_AgentCell")]
        public string DEST_AgentCell { get; set; }
        [Column("DEST_AlternateCell")]
        public string DEST_AlternateCell { get; set; }
        [Column("OA_AGENT")]
        public string OA_AGENT { get; set; }
        [Column("DA_AGENT")]
        public string DA_AGENT { get; set; }
        [Column("PORT_AGENT")]
        public string PORT_AGENT { get; set; }
        [Column("INLAND_AGENT")]
        public string INLAND_AGENT { get; set; }


        [Column("BLUEBARK")]
        public string BLUEBARK { get; set; }

        [Column("NTS")]
        public string NTS { get; set; }

        [Column("CONV")]
        public string CONV { get; set; }

        [Column("ESTIMATED_WEIGHT")]
        public int? ESTIMATED_WEIGHT { get; set; }

        [Column("ACTUAL_WEIGHT")]
        public int? ACTUAL_WEIGHT { get; set; }


    }
}
