namespace Suddath.Helix.JobMgmt.Infrastructure.Constants
{
    public static class PaymentRequestStatus
    {
        public static string QUEUED = "QUEUED";
        public static string APPROVED_BY_THE_GOVERNMENT = "APPROVED_BY_GOV";
        public static string APPROVED_BY_HOMEFRONT = "APPROVED_BY_HF";
        public static string DENIED_BY_THE_GOVERNMENT = "DENIED_BY_GOV";
        public static string GOVERNMENT_PAID_TO_TPPS = "GOV_PAID_TO_TPPS";
        public static string GOVERNMENT_SENT_TO_TPPS = "GOV_SENT_TO_TPPS";
        public static string RECEIVED_BY_TPPS = "RECEIVED_BY_TPPS";
        public static string REJECTED_BY_HOMEFRONT = "REJECTED_BY_HF";
        public static string SUBMITTED_TO_THE_GOVERMENT = "SUBMITTED_TO_GOV";
        public static string SUBMITTED_TO_HOMEFRONT = "SUBMITTED_TO_HF";
        public static string TPPS_PAID_TO_ARC = "TPPS_PAID_TO_ARC";
    }
}