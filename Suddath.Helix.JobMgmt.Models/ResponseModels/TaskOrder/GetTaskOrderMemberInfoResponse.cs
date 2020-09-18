using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderMemberInfoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string PrimaryContactMethod { get; set; }
        public string SecondaryContactMethod { get; set; }
        public AddressDto CurrentAddress { get; set; }
        public AddressDto DeliveryAddress { get; set; }
        public string OriginDutyStationName { get; set; }
        public string OriginDutyStationLocation { get; set; }
        public string DestinationDutyStationName { get; set; }
        public string DestinationDutyStationLocation { get; set; }
        public DateTime? GovtCreatedDateTime { get; set; }
        public string ConfirmationNumber { get; set; }
        public string ServiceMemberIdentifier { get; set; }
        public string DODIdentifier { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public DateTime? ReportToDestinationDateTime { get; set; }
        public string PrimaryPhoneNumberType { get; set; }
        public string SecondaryEmailAddress { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string SecondaryPhoneNumberType { get; set; }
    }
}