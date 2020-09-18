using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public class ApprovalMapperProfile : Profile
    {
        public ApprovalMapperProfile()
        {
            CreateMap<ServiceItem, GetTaskOrderCrateApprovalResponse>()
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId))
                .ForMember(dest => dest.ServiceCode, opt => opt.MapFrom(src => src.RequestedServiceCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ServiceItemStatusIdentifier))
                .ForMember(dest => dest.ShipmentNumber, opt => opt.MapFrom(src => src.RequestedServiceIdentifier))
                .ForMember(dest => dest.CrateDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.CrateLength == null ? 0 : (decimal)src.CrateLength.Value / 1000))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.CrateWidth == null ? 0 : (decimal)src.CrateWidth.Value / 1000))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.CrateHeight == null ? 0 : (decimal)src.CrateHeight.Value / 1000));
            CreateMap<ServiceItem, GetTaskOrderShuttleApprovalResponse>()
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId))
                .ForMember(dest => dest.ServiceCode, opt => opt.MapFrom(src => src.RequestedServiceCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ServiceItemStatusIdentifier))
                .ForMember(dest => dest.ShipmentNumber, opt => opt.MapFrom(src => src.RequestedServiceIdentifier))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ShuttlePickupAddress));
            CreateMap<ServiceItem, GetTaskOrderStorageApprovalResponse>()
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId))
                .ForMember(dest => dest.ServiceCode, opt => opt.MapFrom(src => src.RequestedServiceCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ServiceItemStatusIdentifier))
                .ForMember(dest => dest.ShipmentNumber, opt => opt.MapFrom(src => src.RequestedServiceIdentifier));
            CreateMap<ServiceItem, GetTaskOrderServiceItemApprovalResponse>();
            CreateMap<ServiceItem, ApprovalDto>()
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.CrateLength.HasValue ? (decimal)src.CrateLength.Value / 1000 : (decimal?)null))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.CrateHeight.HasValue ? (decimal)src.CrateHeight.Value / 1000 : (decimal?)null))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.CrateWidth.HasValue ? (decimal)src.CrateWidth.Value / 1000 : (decimal?)null))
                .ForMember(dest => dest.ServiceCode, opt => opt.MapFrom(src => src.RequestedServiceCode))
                .ForMember(dest => dest.CrateDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ShuttlePickupAddress))
                .ForMember(dest => dest.ShipmentNumber, opt => opt.MapFrom(src => src.RequestedServiceIdentifier));
            CreateMap<ApprovalDto, ServiceItem>()
                .ForMember(dest => dest.CrateLength, opt => opt.MapFrom(src => src.Length * 1000))
                .ForMember(dest => dest.CrateHeight, opt => opt.MapFrom(src => src.Height * 1000))
                .ForMember(dest => dest.CrateWidth, opt => opt.MapFrom(src => src.Width * 1000))
                .ForMember(dest => dest.RequestedServiceCode, opt => opt.MapFrom(src => src.ServiceCode))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CrateDescription))
                .ForMember(dest => dest.ShuttlePickupAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.RequestedServiceIdentifier, opt => opt.MapFrom(src => src.ShipmentNumber));
            CreateMap<ApprovalDto, GetTaskOrderCrateApprovalResponse>();
            CreateMap<ApprovalDto, GetTaskOrderShuttleApprovalResponse>();
            CreateMap<ApprovalDto, GetTaskOrderServiceItemApprovalResponse>();
        }
    }
}