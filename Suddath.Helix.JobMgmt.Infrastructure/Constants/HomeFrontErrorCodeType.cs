namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class HomeFrontErrorCodeType
    {
        //TODO: Get real values from Homefront for these. Making them up for now.
        public const string DUPLICATE_TASK_ORDER = "DuplicateTaskOrder";

        public const string DUPLICATE_SHIPMENT = "DuplicateShipment";

        public const string DUPLICATE_SERVICE_ITEM = "DuplicateServiceItem";

        public const string NOT_FOUND_SHIPMENT = "ShipmentNotFound";

        public const string TASK_ORDER_MISSING_ADDRESSES = "TaskOrderMissingAddresses";

        public const string SERVICE_MEMBER_NOT_FOUND = "ServiceMemberNotFound";
    }
}