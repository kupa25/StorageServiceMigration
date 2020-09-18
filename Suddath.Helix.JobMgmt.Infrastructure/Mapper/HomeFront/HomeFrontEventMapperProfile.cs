using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.ServiceItem;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.ToHomeFront.CoordinatorAssigned;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.ToHomeFront.PaymentRequested;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.ToHomeFront.ServiceItemRequestedOrResubmitted;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public class HomeFrontEventMapperProfile : Profile
    {
        public HomeFrontEventMapperProfile()
        {
            CreateMap<JobContact, CoordinatorAssignedEventDto>()
                .ForMember(dest => dest.TaskOrderIdentifier, opt => opt.MapFrom(src => src.Job.TaskOrder.TaskOrderIdentifier))
                 .ForPath(dest => dest.Coordinator.CoordinatorIdentifier, opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.Coordinator.Email, opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.Coordinator.TypeCode, opt => opt.MapFrom(src => "Move Coordinator"))
                ;

            CreateMap<ServiceItem, RequestedServiceItemDto>()
                .ForMember(dest => dest.RequestedServiceId, opt => opt.MapFrom(src => src.RequestedServiceIdentifier))
                .ForMember(dest => dest.TaskOrderIdentifier, opt => opt.MapFrom(src => src.Job.TaskOrderIdentifier))
                .ForMember(dest => dest.ShipmentIdentifier, opt => opt.MapFrom(src => src.SuperServiceOrder != null ? src.SuperServiceOrder.ShipmentIdentifier : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ServiceItemStatusIdentifier))
                .ForPath(dest => dest.Crate, opt => opt.MapFrom(src => src))
                .ForPath(dest => dest.Item, opt => opt.MapFrom(src => src))
                ;

            CreateMap<ServiceItem, CrateDto>()
                .ForPath(dest => dest.ServiceItemDimension.Id, opt => opt.MapFrom(src => src.CrateDimensionIdentifier))
                .ForPath(dest => dest.ServiceItemDimension.Length, opt => opt.MapFrom(src => src.CrateLength))
                .ForPath(dest => dest.ServiceItemDimension.Width, opt => opt.MapFrom(src => src.CrateWidth))
                .ForPath(dest => dest.ServiceItemDimension.Height, opt => opt.MapFrom(src => src.CrateHeight))
                ;

            CreateMap<ServiceItem, ItemDto>()
                .ForPath(dest => dest.ServiceItemDimension.Id, opt => opt.MapFrom(src => src.ItemDimensionIdentifier))
                .ForPath(dest => dest.ServiceItemDimension.Length, opt => opt.MapFrom(src => src.ItemLength))
                .ForPath(dest => dest.ServiceItemDimension.Width, opt => opt.MapFrom(src => src.ItemWidth))
                .ForPath(dest => dest.ServiceItemDimension.Height, opt => opt.MapFrom(src => src.ItemHeight))
                ;

            CreateMap<PaymentRequest, PaymentRequestedEventDto>()
                .ForPath(dest => dest.PaymentRequestIdentifier, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForPath(dest => dest.RequestedDateTime, opt => opt.MapFrom(src => src.PaymentRequestDateTime))
                .ForPath(dest => dest.FinalIndicator, opt => opt.MapFrom(src => src.IsFinished))
                .ForPath(dest => dest.RequestedTotalAmountInDollars, opt => opt.MapFrom(src => src.RequestedTotalAmountUSD))
                .ForPath(dest => dest.PaymentServiceItems, opt => opt.MapFrom(src => src.ServiceItem))
                .ForPath(dest => dest.Documents, opt => opt.MapFrom(src => new List<PaymentRequestedDocumentDto>()))
                .ForPath(dest => dest.TaskOrderIdentifier, opt => opt.MapFrom(src => src.ServiceItem.FirstOrDefault().Job.TaskOrderIdentifier))
                ;

            CreateMap<ServiceItem, PaymentServiceItemDto>()
                .ForPath(dest => dest.HomeFrontServiceItemIdentifier, opt => opt.MapFrom(src => src.ServiceItemIdentifier))
                .ForPath(dest => dest.RequestedPriceInDollars, opt => opt.MapFrom(src => src.RequestedPriceUSD))
                ;

            CreateMap<HfAddressDto, Address>()
                .ForPath(dest => dest.State, opt => opt.MapFrom(src => src.StateOrProvince))
                .ForPath(dest => dest.Address1, opt => opt.MapFrom(src => src.AddressLine1))
                .ForPath(dest => dest.Address2, opt => opt.MapFrom(src => src.AddressLine2))
                .ForPath(dest => dest.Address3, opt => opt.MapFrom(src => src.AddressLine3))
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.County))
                .ForPath(dest => dest.CountryCode3, opt => opt.MapFrom(src => src.CountryCode)).ReverseMap()
                ;
        }
    }
}