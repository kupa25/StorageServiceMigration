using System;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class ServiceItemCode
    {
        #region ServiceCodes
        public const string COUNSELING_SERVICES = "CS";
        public const string DOMESTIC_CRATING = "DCRT";
        public const string DOMESTIC_CRATING_STAND_ALONE = "DCRTSA";
        public const string DOMESTIC_DESTINATION_FIRST_DAY_SIT = "DDFSIT";
        public const string DOMESTIC_DESTINATION_ADDITIONAL_SIT = "DDASIT";
        public const string DOMESTIC_DESTINATION_PRICE = "DDP";
        public const string DOMESTIC_DESTINATION_SHUTTLE_PRICE = "DDSHUT";
        public const string DOMESTIC_DESTINATION_SIT_DELIVERY = "DDDSIT";
        public const string DOMESTIC_HAUL_AWAY_BOAT_FACTOR = "DBHF";
        public const string DOMESTIC_LINE_HAUL = "DLH";
        public const string DOMESTIC_MOBILE_HOME_FACTOR = "DMHF";
        public const string DOMESTIC_NTS_PACKING_FACTOR = "DNPKF";
        public const string DOMESTIC_ORIGIN_1ST_DAY_SIT = "DOFSIT";
        public const string DOMESTIC_ORIGIN_ADDITIONAL_SIT = "DOASIT";
        public const string DOMESTIC_ORIGIN_PRICE = "DOP";
        public const string DOMESTIC_ORIGIN_SHUTTLE_SERVICE = "DOSHUT";
        public const string DOMESTIC_ORIGIN_SIT_PICKUP = "DOPSIT";
        public const string DOMESTIC_PACKING = "DPK";
        public const string DOMESTIC_SHORT_HAUL = "DSH";
        public const string DOMESTIC_TOW_AWAY_BOAT_FACTOR = "DBTF";
        public const string DOMESTIC_UNCRATING = "DUCRT";
        public const string DOMESTIC_UNPACKING = "DUPK";
        public const string FUEL_SURCHARGE = "FSC";
        public const string INTERNATIONAL_C_O_SHIPPING_AND_LH = "ICOLH";
        public const string INTERNATIONAL_C_O_UB = "ICOUB";
        public const string INTERNATIONAL_CRATING = "ICRT";
        public const string INTERNATIONAL_CRATING_STANDALONE = "ICRTSA";
        public const string INTERNATIONAL_DESTINATION_1ST_DAY_SIT = "IDFSIT";
        public const string INTERNATIONAL_DESTINATION_ADDITIONAL_DAY_SIT = "IDASIT";
        public const string INTERNATIONAL_DESTINATION_SHUTTLE_SERVICE = "IDSHUT";
        public const string INTERNATIONAL_DESTINATION_SIT_DELIVERY = "IDDSIT";
        public const string INTERNATIONAL_HAUL_AWAY_BOAT_FACTOR = "IBHF";
        public const string INTERNATIONAL_HHG_PACK = "IHPK";
        public const string INTERNATIONAL_HHG_UNPACK = "IHUPK";
        public const string INTERNATIONAL_NTS_PACKING_FACTOR = "INPKF";
        public const string INTERNATIONAL_O_C_UB = "IOCUB";
        public const string INTERNATIONAL_O_O_SHIPPING_LH = "IOOLH";
        public const string INTERNATIONAL_O_O_UB = "IOOUB";
        public const string INTERNATIONAL_ORIGIN_1ST_DAY_SIT = "IOFSIT";
        public const string INTERNATIONAL_ORIGIN_ADDITIONAL_DAY_SIT = "IOASIT";
        public const string INTERNATIONAL_ORIGIN_SHUTTLE_SERVICE = "IOSHUT";
        public const string INTERNATIONAL_ORIGIN_SIT_PICKUP = "IOPSIT";
        public const string INTERNATIONAL_TOW_AWAY_BOAT_FACTOR = "IBTF";
        public const string INTERNATIONAL_UB_PACK = "IUBPK";
        public const string INTERNATIONAL_UB_UNPACK = "IUBUPK";
        public const string INTERNATIONAL_UNCRATING = "IUCRT";
        public const string NON_STANDARD_HHG = "NSTH";
        public const string NON_STANDARD_UB = "NSTUB";
        public const string SHIPMENT_MANAGEMENT_SERVICES = "MS";
        #endregion ServiceCodes

        public static String[] CrateServiceCodes =
        {
            DOMESTIC_CRATING
            , DOMESTIC_CRATING_STAND_ALONE
            , DOMESTIC_UNCRATING
            , INTERNATIONAL_CRATING
            , INTERNATIONAL_CRATING_STANDALONE
            , INTERNATIONAL_UNCRATING
        };

        public static String[] ShuttleServiceCodes =
        {
            DOMESTIC_DESTINATION_SHUTTLE_PRICE
            , DOMESTIC_ORIGIN_SHUTTLE_SERVICE
            , INTERNATIONAL_DESTINATION_SHUTTLE_SERVICE
            , INTERNATIONAL_ORIGIN_SHUTTLE_SERVICE
        };

        public static String[] StorageServiceCodes =
        {
            DOMESTIC_DESTINATION_ADDITIONAL_SIT
            , DOMESTIC_DESTINATION_SIT_DELIVERY
            , DOMESTIC_DESTINATION_FIRST_DAY_SIT
            , DOMESTIC_ORIGIN_ADDITIONAL_SIT
            , DOMESTIC_ORIGIN_1ST_DAY_SIT
            , DOMESTIC_ORIGIN_SIT_PICKUP
            , INTERNATIONAL_DESTINATION_ADDITIONAL_DAY_SIT
            , INTERNATIONAL_DESTINATION_SIT_DELIVERY
            , INTERNATIONAL_DESTINATION_1ST_DAY_SIT
            , INTERNATIONAL_ORIGIN_ADDITIONAL_DAY_SIT
            , INTERNATIONAL_ORIGIN_1ST_DAY_SIT
        };

        public static bool IsCrateServiceCode(string code) => CrateServiceCodes.Contains(code);
        public static bool IsShuttleServiceCode(string code) => ShuttleServiceCodes.Contains(code);
        public static bool IsStorageServiceCode(string code) => StorageServiceCodes.Contains(code);
    }
}
