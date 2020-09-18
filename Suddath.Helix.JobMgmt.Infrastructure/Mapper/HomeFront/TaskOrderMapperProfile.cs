using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Infrastructure.Extensions;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public class TaskOrderMapperProfile : Profile
    {
        public TaskOrderMapperProfile()
        {
            CreateMap<TaskOrderDto, Job2Dto>()
                .ForMember(dest => dest.Transferee, opt => opt.MapFrom(src => src.ServiceMember))
                .ForPath(dest => dest.Job.Status, opt => opt.MapFrom(src => JobStatusDisplayName.ACTIVE_BOOKED))
                .ForPath(dest => dest.Transferee.IsVip, opt => opt.MapFrom(src => src.VIPIndicator))
                .ForPath(dest => dest.JobInfo, opt => opt.MapFrom(src => ModelTranslations.ToAriveAddresses(src.ServiceMember.ContactInfo.CurrentAddress, src.ServiceMember.ContactInfo.DestinationAddress)))

                .ForPath(dest => dest.Job.BillTo.Id, opt => opt.MapFrom(src =>
                    AccountEntityId.AMERICAN_ROLL_ON_ROLL_OFF))
                .ForPath(dest => dest.Job.BillTo.Name, opt => opt.MapFrom(src =>
                    AccountEntityName.AMERICAN_ROLL_ON_ROLL_OFF))
                .ForPath(dest => dest.Job.BillToLabel, opt => opt.MapFrom(src =>
                    AccountEntityName.AMERICAN_ROLL_ON_ROLL_OFF))

                .ForPath(dest => dest.Job.Account.Id, opt => opt.MapFrom(src =>
                    AccountEntityId.AMERICAN_ROLL_ON_ROLL_OFF))
                .ForPath(dest => dest.Job.Account.Name, opt => opt.MapFrom(src =>
                    AccountEntityName.AMERICAN_ROLL_ON_ROLL_OFF))
                .ForPath(dest => dest.Job.AccountLabel, opt => opt.MapFrom(src =>
                    AccountEntityName.AMERICAN_ROLL_ON_ROLL_OFF))

                 .ForPath(dest => dest.Job.Booker.Id, opt => opt.MapFrom(src =>
                    VendorId.SUDDATH_GOVERNMENT_SERVICES))
                .ForPath(dest => dest.Job.Booker.Name, opt => opt.MapFrom(src =>
                    VendorName.SUDDATH_GOVERNMENT_SERVICES))
                .ForPath(dest => dest.Job.BookerLabel, opt => opt.MapFrom(src =>
                    VendorName.SUDDATH_GOVERNMENT_SERVICES))

                .ForPath(dest => dest.Job.RevenueType, opt => opt.MapFrom(src =>
                    RevenueType.SUDDATH))
                .ForPath(dest => dest.Job.BranchName, opt => opt.MapFrom(src =>
                    BranchIdentifier.SUDDATH_GOVERNMENT_SERVICES))
                .ForPath(dest => dest.Job.MoveType, opt => opt.MapFrom(src =>
                    MoveType.DOOR_TO_DOOR))
                .ForPath(dest => dest.Job.BillToType, opt => opt.MapFrom(src => EntityType.ACCOUNT_ENTITY))
                .ForPath(dest => dest.Job.AuthPoNum, opt => opt.MapFrom(src => src.TaskOrderIdentifier))
                .ForPath(dest => dest.Job.JobSource, opt => opt.MapFrom(src => JobSourceType.HOME_FRONT))
                ;

            CreateMap<Job2Dto, TaskOrderResponse>()
                     .ForMember(dest => dest.TaskOrderIdentifier, opt => opt.MapFrom(src => src.Job.JobId))
                     .ForMember(dest => dest.JobSource, opt => opt.MapFrom(src => src.Job.JobSource))
                     .ForMember(dest => dest.VIPIndicator, opt => opt.MapFrom(src => src.Transferee.IsVip))
                     .ForMember(dest => dest.ServiceMember, opt => opt.MapFrom(src => src.Transferee))
                     .ForMember(dest => dest.AddressInfo, opt => opt.MapFrom(src => ModelTranslations.ToAddresseInfoResponse(src.JobInfo)))
                     .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.Job.DateCreated))
                     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Job.Status))
                     .ForMember(dest => dest.TaskOrderId, opt => opt.MapFrom(src => src.Job.JobId))
                     .ForMember(dest => dest.ServiceMemberName, opt => opt.MapFrom(src => src.Transferee.LastName + ", " + src.Transferee.FirstName))
                     .ForMember(dest => dest.OriginAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(ModelTranslations.ToAddresseInfoResponse(src.JobInfo).OriginAddress)))
                     .ForMember(dest => dest.DestinationAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(ModelTranslations.ToAddresseInfoResponse(src.JobInfo).DestinationAddress)))
                     ;

            CreateMap<TaskOrder, TaskOrderResponse>()
                    .ForMember(dest => dest.TaskOrderIdentifier, opt => opt.MapFrom(src => src.JobId))
                    .ForMember(dest => dest.JobSource, opt => opt.MapFrom(src => JobSourceType.HOME_FRONT))
                    .ForMember(dest => dest.VIPIndicator, opt => opt.MapFrom(src => src.Job.Transferee.IsVip))
                    .ForMember(dest => dest.ServiceMember, opt => opt.MapFrom(src => src.Job.Transferee))
                    .ForMember(dest => dest.AddressInfo, opt => opt.MapFrom(src => ModelTranslations.ToTaskOrderAddressInfoResponse(src.Job)))
                    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.Job.DateCreated))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => JobStatusDisplayName.ACTIVE_BOOKED))
                    .ForMember(dest => dest.MovePriorityType, opt => opt.MapFrom(src => src.MovePriority))
                    .ForMember(dest => dest.MilitaryBranch, opt => opt.MapFrom(src => src.ServiceMemberBranch))
                    .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.ServiceMemberRank))
                    .ForMember(dest => dest.TaskOrderId, opt => opt.MapFrom(src => src.JobId))
                    .ForMember(dest => dest.ServiceMemberName, opt => opt.MapFrom(src => src.Job.Transferee.LastName + ", " + src.Job.Transferee.FirstName))
                    .ForMember(dest => dest.OriginAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.Job.OriginAddress)))
                    .ForMember(dest => dest.DestinationAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.Job.DestinationAddress)))
                    .ForMember(dest => dest.ServiceMember, opt => opt.MapFrom(src => src.Job.Transferee))
                    ;

            CreateMap<ServiceMemberDto, TransfereeDto>()
                .ForMember(dest => dest.Emails, opt => opt.MapFrom(src => ModelTranslations.ToEmailDto(src.ContactInfo)))
                .ForMember(dest => dest.OriginPhones, opt => opt.MapFrom(src => ModelTranslations.ToPhoneDto(src.ContactInfo.ContactNumber)));

            CreateMap<TransfereeDto, ServiceMemberDto>()
                .ForMember(dest => dest.ContactInfo, opt => opt.MapFrom(src => ModelTranslations.ToContactInfoDto(src)));

            CreateMap<Transferee, ServiceMemberDto>()
                .ForMember(dest => dest.ContactInfo, opt => opt.MapFrom(src => ModelTranslations.ToContactInfoDto(src)));

            CreateMap<ContactNumberDto, TransfereePhone>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => ModelTranslations.ToPhone(src)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<TransfereePhone, ContactNumberDto>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone.NationalNumber))
                .ForMember(dest => dest.PhoneNumberExtension, opt => opt.MapFrom(src => src.Phone.Extension))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.Phone.CountryCode))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<TransfereePhone, Phone>()
                .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => src.Phone.NationalNumber))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.Phone.Extension))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<ContactNumberDto, PhoneDto>()
                .ForMember(dest => dest.NationalNumber, opt => opt.MapFrom(src => PhoneExtensions.ToUnformatted(src.PhoneNumber)))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.PhoneNumberExtension))
                .ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => src.Type));

            CreateMap<PhoneDto, ContactNumberDto>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.NationalNumber))
                .ForMember(dest => dest.PhoneNumberExtension, opt => opt.MapFrom(src => src.Extension))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.PhoneType));

            CreateMap<DutyStationAddressDto, AddressDto>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address2))
                .ForMember(dest => dest.AdditionalAddressInfo, opt => opt.MapFrom(src =>
                    src.AddressLine2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address3))
                .ForMember(dest => dest.Display, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Display))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StateOrProvince))
                .ForMember(dest => dest.CountryCode3, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.Longitude, opt => opt.Ignore())
                .ForMember(dest => dest.Latitude, opt => opt.Ignore())
                ;

            CreateMap<HfAddressDto, AddressDto>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address2))
                .ForMember(dest => dest.AdditionalAddressInfo, opt => opt.MapFrom(src =>
                    src.AddressLine2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address3))
                .ForMember(dest => dest.Display, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Display))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StateOrProvince))
                .ForMember(dest => dest.CountryCode3, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.Longitude, opt => opt.Ignore())
                .ForMember(dest => dest.Latitude, opt => opt.Ignore())
                ;

            CreateMap<AddressDto, DutyStationAddressDto>()
                .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.Address1))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.Address2))
                .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.Address3))
                .ForMember(dest => dest.StateOrProvince, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode3))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                ;

            CreateMap<AddressDto, HfAddressDto>()
                .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.Address1))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.Address2))
                .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.Address3))
                .ForMember(dest => dest.StateOrProvince, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode3))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                ;

            CreateMap<Address, HfAddressDto>()
                .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.Address1))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.Address2))
                .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.Address3))
                .ForMember(dest => dest.StateOrProvince, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode3))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                ;

            CreateMap<TaskOrderDto, TaskOrder>()
                .ForPath(dest => dest.OriginDutyStationName, opt => opt.MapFrom(src => src.OriginDutyStationAddress.DutyStationName))
                .ForPath(dest => dest.DestinationDutyStationName, opt => opt.MapFrom(src => src.DestinationDutyStationAddress.DutyStationName))
                .ForMember(dest => dest.MovePriority, opt => opt.MapFrom(src => src.MovePriorityType))
                .ForMember(dest => dest.TaskOrderStatus, opt => opt.MapFrom(src => "Active"))
                .ForPath(dest => dest.ServiceMemberBranch, opt => opt.MapFrom(src => src.ServiceMember.ServiceBranch))
                .ForPath(dest => dest.ServiceMemberIdentifier, opt => opt.MapFrom(src => src.ServiceMember.ServiceMemberIdentifier))
                .ForPath(dest => dest.ServiceMemberDODIdentifier, opt => opt.MapFrom(src => src.ServiceMember.DODIdentifier))
                .ForPath(dest => dest.ServiceMemberPrimaryCommunicationPreference, opt => opt.MapFrom(src => src.ServiceMember.ContactInfo.PrimaryCommunicationPref))
                .ForPath(dest => dest.ServiceMemberSecondaryCommunicationPreference, opt => opt.MapFrom(src => src.ServiceMember.ContactInfo.SecondaryCommunicationPref))
                .ForPath(dest => dest.ServiceMemberRank, opt => opt.MapFrom(src => src.ServiceMember.Rank));
            ;

            CreateMap<DutyStationAddressDto, AddressDto>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address1))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address2))
                .ForMember(dest => dest.AdditionalAddressInfo, opt => opt.MapFrom(src =>
                    src.AddressLine2))
                .ForMember(dest => dest.Address3, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Address3))
                .ForMember(dest => dest.Display, opt => opt.MapFrom(src =>
                    ModelTranslations.ToGoogleAddressFormattedList(src).Display))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StateOrProvince))
                .ForMember(dest => dest.CountryCode3, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.Longitude, opt => opt.Ignore())
                .ForMember(dest => dest.Latitude, opt => opt.Ignore())
                ;

            CreateMap<DutyStationAddressDto, Address>()
                .ForMember(dest => dest.Longitude, opt => opt.Ignore())
                .ForMember(dest => dest.Latitude, opt => opt.Ignore())
                ;

            CreateMap<EntitlementDto, Entitlement>()
                .ForMember(dest => dest.AuthorizedWeightLb, opt => opt.MapFrom(src => src.AuthorizedWeight))
                .ForMember(dest => dest.ProGearWeightLb, opt => opt.MapFrom(src => src.ProGearWeight))
                .ForMember(dest => dest.ProGearWeightSpouseLb, opt => opt.MapFrom(src => src.ProGearWeightSpouse))
                .ForMember(dest => dest.TotalWeightLb, opt => opt.MapFrom(src => src.TotalWeight))
                .ForMember(dest => dest.IsDependentsAuthorized, opt => opt.MapFrom(src => src.DependentsAuthorizedIndicator))
                .ForMember(dest => dest.IsNonTemporaryStorage, opt => opt.MapFrom(src => src.NonTemporaryStorageIndicator))
                .ForMember(dest => dest.IsPrivatelyOwnedVehicle, opt => opt.MapFrom(src => src.PrivatelyOwnedVehicleIndicator))
                .ForMember(dest => dest.StorageInTransitDays, opt => opt.MapFrom(src => src.StorageInTransitDays))
                .ForMember(dest => dest.TotalDependents, opt => opt.MapFrom(src => src.TotalDependents))
                .ForMember(dest => dest.EffectiveStartDateTime, opt => opt.MapFrom(src => src.EffectiveStartDateTime))
                .ForMember(dest => dest.EffectiveEndDateTime, opt => opt.MapFrom(src => src.EffectiveEndDateTime))
                ;

            CreateMap<ServiceItemDto, ServiceItem>()
                .ForMember(dest => dest.ServiceItemStatusIdentifier, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.RequestedServiceIdentifier, opt => opt.MapFrom(src => src.RequestedServiceId))
                .ForMember(dest => dest.ItemDimensionIdentifier, opt => opt.MapFrom(src => src.Item.ServiceItemDimension.Id))
                .ForMember(dest => dest.ItemLength, opt => opt.MapFrom(src => src.Item.ServiceItemDimension.Length))
                .ForMember(dest => dest.ItemWidth, opt => opt.MapFrom(src => src.Item.ServiceItemDimension.Width))
                .ForMember(dest => dest.ItemHeight, opt => opt.MapFrom(src => src.Item.ServiceItemDimension.Height))
                .ForMember(dest => dest.CrateDimensionIdentifier, opt => opt.MapFrom(src => src.Crate.ServiceItemDimension.Id))
                .ForMember(dest => dest.CrateLength, opt => opt.MapFrom(src => src.Crate.ServiceItemDimension.Length))
                .ForMember(dest => dest.CrateWidth, opt => opt.MapFrom(src => src.Crate.ServiceItemDimension.Width))
                .ForMember(dest => dest.CrateHeight, opt => opt.MapFrom(src => src.Crate.ServiceItemDimension.Height))
                ;

            CreateMap<Job, GetTaskOrderMemberInfoResponse>()
                .ForMember(dest => dest.ConfirmationNumber, opt => opt.MapFrom(src => "DemoConfirmation#"))
                .ForMember(dest => dest.DestinationDutyStationName, opt => opt.MapFrom(src => src.TaskOrder.DestinationDutyStationName))
                .ForMember(dest => dest.OriginDutyStationName, opt => opt.MapFrom(src => src.TaskOrder.OriginDutyStationName))
                .ForMember(dest => dest.DODIdentifier, opt => opt.MapFrom(src => src.TaskOrder.TaskOrderIdentifier))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Transferee.Title))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Transferee.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Transferee.LastName))
                .ForMember(dest => dest.PrimaryContactMethod, opt => opt.MapFrom(src => src.TaskOrder.PreferredContactMethod))
                .ForMember(dest => dest.PrimaryEmailAddress, opt => opt.MapFrom(src => src.Transferee.Email.FirstOrDefault().Value))
                .ForMember(dest => dest.PrimaryPhoneNumber, opt => opt.MapFrom(src => src.Transferee.TransfereePhone
                    .OrderByDescending(p => p.PhoneId)
                    .Select(p => new { Phone = string.Concat(p.Phone.DialCode, p.Phone.NationalNumber) })
                    .FirstOrDefault().Phone ?? string.Empty))
                .ForMember(dest => dest.CurrentAddress, opt => opt.MapFrom(src => src.OriginAddress))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DestinationAddress))
                .ForMember(dest => dest.OriginDutyStationLocation,
                        opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.TaskOrder.OriginDutyStationAddress.City, src.TaskOrder.OriginDutyStationAddress.State, src.TaskOrder.OriginDutyStationAddress.CountryCode3)))
                .ForMember(dest => dest.DestinationDutyStationLocation,
                        opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.TaskOrder.DestinationDutyStationAddress.City, src.TaskOrder.DestinationDutyStationAddress.State, src.TaskOrder.DestinationDutyStationAddress.CountryCode3)))

                ;
        }
    }
}