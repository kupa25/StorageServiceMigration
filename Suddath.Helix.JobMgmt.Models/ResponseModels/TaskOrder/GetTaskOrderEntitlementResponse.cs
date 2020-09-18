using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderEntitlementResponse
    {
        public int JobId { get; set; }
        public int AuthorizedWeightLb { get; set; }
        public int ProGearWeightLb { get; set; }
        public int ProGearWeightSpouseLb { get; set; }
        public int TotalWeightLb { get; set; }
        public bool IsDependentsAuthorized { get; set; }
        public int TotalDependents { get; set; }
        public bool IsNonTemporaryStorage { get; set; }
        public bool IsPrivatelyOwnedVehicle { get; set; }
        public int AuthorizedSitDays { get; set; }
        public DateTime? EffectiveStartDateTime { get; set; }
        public DateTime? EffectiveEndDateTime { get; set; }

        public bool IsEntitlementOrdered { get; set; }
        public DateTime? CounseledIssueDate { get; set; }
        public DateTime? RequestedCounselingDate { get; set; }
        public DateTime? ScheduledCounselingDate { get; set; }
        public string EntitlementCounselor { get; set; }
        public DateTime? CounselingCompletedDate { get; set; }
        public string OriginGbloc { get; set; }
        public string OriginPpso { get; set; }
        public string DestinationGbloc { get; set; }
        public string DestinationPpso { get; set; }
        public int EstimatedWeight { get; set; }
        public int TotalNumberOfShipments { get; set; }
        public string TravelAuthorizationNumber { get; set; }
        public DateTime? TravelAuthorizationDate { get; set; }
        public string TravelAuthorizationIssueHeadquarters { get; set; }
        public DateTime? NtsExpirationDate { get; set; }
        public string NtsReleaseInformation { get; set; }
        public string Remarks { get; set; }
    }
}