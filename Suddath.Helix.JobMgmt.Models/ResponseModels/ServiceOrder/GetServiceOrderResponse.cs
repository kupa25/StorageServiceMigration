using System;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder
{
    public interface IServiceOrderBaseResponse
    {
        int ServiceOrderId { get; set; }
        int? VendorId { get; set; }
        string VendorName { get; set; }
        decimal? QuotedRate { get; set; }
        string QuotedUnit { get; set; }
        string Currency { get; set; }
        decimal? QuoteTotal { get; set; }
        string QuoteReferenceNumber { get; set; }
        DateTime? LastFreeDateAtPort { get; set; }
        DateTime? LastFreeDate { get; set; }
        int? CarrierVendorId { get; set; }
        string CarrierVendorName { get; set; }
        DateTime? BookingDate { get; set; }
        string XTNNumber { get; set; }
        string ITNNumber { get; set; }
    }

    public class ServiceOrderBaseResponse : IServiceOrderBaseResponse
    {
        public ServiceOrderBaseResponse()
        {
        }

        public int ServiceOrderId { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal? QuotedRate { get; set; }
        public string QuotedUnit { get; set; }
        public string Currency { get; set; }
        public decimal? QuoteTotal { get; set; }
        public string QuoteReferenceNumber { get; set; }
        public DateTime? LastFreeDateAtPort { get; set; }
        public DateTime? LastFreeDate { get; set; }
        public int? CarrierVendorId { get; set; }
        public string CarrierVendorName { get; set; }
        public DateTime? BookingDate { get; set; }
        public string XTNNumber { get; set; }
        public string ITNNumber { get; set; }
        public string SuperServiceName { get; set; }
        public string SuperServiceModeName { get; set; }
    }

    public class GetServiceOrderAirFreightResponse : ServiceOrderBaseResponse
    {
        public GetServiceOrderAirFreightResponse()
        {
        }

        public string MasterAwbNumber { get; set; }
        public string HouseAirwayBillNumber { get; set; }
        public DateTime? ITNFiledDate { get; set; }
        public string EINNumber { get; set; }
        public string CountryOfIssue { get; set; }
    }

    public class GetServiceOrderLineHaulResponse : ServiceOrderBaseResponse
    {
        public decimal? FSC { get; set; }
        public decimal? ChassisCost { get; set; }
        public bool IsChassisProvided { get; set; }
        public string AgentType { get; set; }
        public string LineHaulType { get; set; }
        public string LocationType { get; set; }
        public DateTime? SendOrderDate { get; set; }
        public DateTime? LiveLoadDate { get; set; }
        public DateTime? LiveUnloadDate { get; set; }
        public DateTime? RequestedPickupDate { get; set; }
        public DateTime? ActualPickupDate { get; set; }
        public DateTime? RequestedDropDate { get; set; }
        public DateTime? ActualDropDate { get; set; }
        public int? FreeDays { get; set; }
        public string DaysType { get; set; }
    }

    public class GetServiceOrderOceanFreightResponse : ServiceOrderBaseResponse
    {
        public string CarrierBookingNumber { get; set; }
        public string NVOCCBookingNumber { get; set; }
        public string MasterBOLNumber { get; set; }
        public string HouseNVOCCBOLNumber { get; set; }
        public DateTimeOffset? ContainerCutoffDateTime { get; set; }
        public DateTimeOffset? DocumentsCutoffDateTime { get; set; }
        public DateTime? EarliestReturnDate { get; set; }
        public string FreightContractorName { get; set; }
        public DateTime? ITNFiledDate { get; set; }
        public string EINNumber { get; set; }
        public string CountryOfIssue { get; set; }
    }

    public class GetServiceOrderCustomsClearanceResponse : ServiceOrderBaseResponse
    {
        public DateTime? CustomsEntryDate { get; set; }
        public DateTime? CustomsReleaseDate { get; set; }
        public DateTime? DocumentOutDate { get; set; }
        public string AgentType { get; set; }
        public string ISFReferenceNumber { get; set; }
        public DateTime? ISFDocumentReceivedDate { get; set; }
        public DateTime? ISFSendDate { get; set; }
        public DateTime? ISFReceivedDate { get; set; }
        public DateTime? ISFMatchDate { get; set; }
    }

    public class GetServiceOrderThirdPartyResponse : ServiceOrderBaseResponse
    {
        public decimal? TotalCrateVolumeCUFT { get; set; }
        public DateTime? RequestedServiceDate { get; set; }
        public DateTime? ActualServiceDate { get; set; }
        public string ContractType { get; set; }
    }

    public class GetServiceOrderOriginAgentResponse : ServiceOrderBaseResponse
    {
        public DateTime? EstimatedPackStartDate { get; set; }
        public DateTime? EstimatedPackEndDate { get; set; }
        public DateTime? EstimatedPackStartTime { get; set; }
        public DateTime? EstimatedPackEndTime { get; set; }
        public DateTime? ActualPackStartDate { get; set; }
        public DateTime? ActualPackEndDate { get; set; }
        public DateTime? EstimatedPickupStartDate { get; set; }
        public DateTime? EstimatedPickupEndDate { get; set; }
        public DateTime? ActualPickupStartDate { get; set; }
        public DateTime? ActualPickupEndDate { get; set; }
        public string CrewLeaderName { get; set; }
        public DateTime? OAInstructionsSentDate { get; set; }
        public DateTime? OAFinalsReceivedDate { get; set; }
        public DateTime? OAGreenLightDate { get; set; }
        public DateTime? OASubmittedToFMCDate { get; set; }
        public int? OAPieceCount { get; set; }
        public bool IsSITAuthorized { get; set; }
        public DateTime? SITInDate { get; set; }
        public DateTime? SITLastAuthorizedDate { get; set; }
        public DateTime? SITOutDate { get; set; }
        public int? SITDaysUsed { get; set; }
        public int? SITDaysAuthorized { get; set; }
        public bool? IsAllDocumentsReceived { get; set; }
        public int? NetWeightLb { get; set; }
        public int? NetWeightKg { get; set; }
    }

    public class GetServiceOrderDestinationAgentResponse : ServiceOrderBaseResponse
    {
        public string CrewLeaderName { get; set; }
        public DateTime? SITInDate { get; set; }
        public DateTime? SITLastAuthorizedDate { get; set; }
        public DateTime? SITOutDate { get; set; }
        public int? SITDaysUsed { get; set; }
        public int? SITDaysAuthorized { get; set; }
        public DateTime? RequestedDeliveryStartDate { get; set; }
        public DateTime? RequestedDeliveryEndDate { get; set; }
        public DateTime? ScheduledDeliveryStartDate { get; set; }
        public DateTime? ScheduledDeliveryEndDate { get; set; }
        public DateTime? ScheduledDeliveryStartTime { get; set; }
        public DateTime? ScheduledDeliveryEndTime { get; set; }
        public DateTime? ActualDeliveryStartDate { get; set; }
        public DateTime? ActualDeliveryEndDate { get; set; }
        public DateTime? ActualUnpackStartDate { get; set; }
        public DateTime? ActualUnpackEndDate { get; set; }
        public DateTime? DocumentsOutDate { get; set; }
        public DateTime? PreAdviceSentDate { get; set; }
        public DateTime? SignedDeliveryDocumentsReceivedDate { get; set; }
        public int? ReweighWeightLb { get; set; }
        public int? TotalWeightDeliveredLb { get; set; }
        public int? RemainingWeightToDeliverLb { get; set; }
        public bool IsPartialDeliveryExists { get; set; }
        public bool IsSITAuthorized { get; set; }
        public string AgentType { get; set; }
    }

    public class GetServiceOrderRoadFreightResponse : ServiceOrderBaseResponse
    {
        public DateTime? ITNFiledDate { get; set; }
        public string EINNumber { get; set; }
        public string CountryOfIssue { get; set; }
    }

    public class GetServiceOrderStorageResponse : ServiceOrderBaseResponse
    {
        public decimal? StorageCostRate { get; set; }
        public string StorageCostUnit { get; set; }
        public decimal? InsuranceCostRate { get; set; }
        public string InsuranceCostUnit { get; set; }
        public DateTime? ActualDateIn { get; set; }
        public DateTime? ActualDateOut { get; set; }
        public string AgentType { get; set; }
    }
}