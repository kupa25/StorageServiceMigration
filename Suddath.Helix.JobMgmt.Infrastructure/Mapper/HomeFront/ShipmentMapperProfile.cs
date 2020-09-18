using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.ShipmentAdded;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public class ShipmentMapperProfile : Profile
    {
        public ShipmentMapperProfile()
        {
            CreateMap<Shipment, GetTaskOrderShipmentInfoResponse>();

            CreateMap<GetTaskOrderShipmentInfoResponse, Shipment>()
               .ForMember(dest => dest.EstimatedWeightLb, opt => opt.MapFrom(src => src.EstimatedWeightLb))
                .ForMember(dest => dest.ShipmentIdentifier, opt => opt.Ignore())
                .ForMember(dest => dest.SuperServiceOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.JobId, opt => opt.Ignore())
                .ForMember(dest => dest.PickupAddressId, opt => opt.Ignore())
                .ForMember(dest => dest.DestinationAddressId, opt => opt.Ignore())
                .ForMember(dest => dest.SecondaryPickupAddressId, opt => opt.Ignore())
                .ForMember(dest => dest.SecondaryDestinationAddressId, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentType, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedPickupDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.RequiredDeliveryDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerRemarks, opt => opt.Ignore())
               ;

            CreateMap<ServiceOrder, GetTaskOrderShipmentServiceResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.Service))
                .ForMember(d => d.ServiceId, opt => opt.MapFrom(src => src.Service.Id))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(d => d.SortOrder, opt => opt.MapFrom(src => src.Service.SortOrder))
                .ForMember(d => d.ServiceAbbreviation, opt => opt.MapFrom(src => src.Service.ServiceAbbreviation));

            CreateMap<SuperServiceOrder, GetTaskOrderShipmentResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => src.SuperService.SuperServiceName))
                .ForMember(d => d.SuperServiceModeName, opt => opt.MapFrom(src => src.SuperServiceMode.ModeName))
                .ForMember(d => d.SuperServiceId, opt => opt.MapFrom(src => src.SuperService.Id))
                .ForMember(d => d.SuperServiceIconName, opt => opt.MapFrom(src => src.SuperService.SuperServiceIconName))
                .ForMember(d => d.Status, opt => opt.MapFrom(src => src.SuperServiceOrderStatusIdentifierNavigation.SuperServiceOrderStatusDisplayName))
                .ForPath(d => d.ServiceOrders, opt => opt.MapFrom(src => src.ServiceOrder))
                .AfterMap((s, resp) =>
                {
                    resp.ServiceOrders = resp.ServiceOrders.OrderBy(x => x.SortOrder);
                })
                ;

            CreateMap<SuperServiceOrder, GetTaskOrderShipmentInfoResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ShipmentIdentifier, opt => opt.MapFrom(src => src.Shipment.ShipmentIdentifier))
                .ForMember(d => d.ShipmentType, opt => opt.MapFrom(src => src.Shipment.ShipmentType))
                .ForMember(d => d.ApprovedDateTime, opt => opt.MapFrom(src => src.Shipment.ApprovedDateTime))
                .ForMember(d => d.RequestedPickupDateTime, opt => opt.MapFrom(src => src.Shipment.RequestedPickupDateTime))
                .ForMember(d => d.RequestedDeliveryDateTime, opt => opt.MapFrom(src => src.Shipment.RequiredDeliveryDateTime)) //TODO: work with UI team to change the prop
                .ForMember(d => d.Remark, opt => opt.MapFrom(src => src.Shipment.CustomerRemarks))
                .ForMember(d => d.ShipmentName, opt => opt.MapFrom(src => src.SuperService.SuperServiceName))
                .ForMember(d => d.ShipmentIconName, opt => opt.MapFrom(src => src.SuperService.SuperServiceIconName))
                ;

            CreateMap<ShipmentAddedDto, ShipmentDto>()
                .ForPath(d => d.SecondaryDestinationAddress, opt => opt.MapFrom(src => src.SecondaryDestinationAddresses.FirstOrDefault()))
                .ForPath(d => d.SecondaryPickupAddress, opt => opt.MapFrom(src => src.SecondaryPickupAddresses.FirstOrDefault()))
                ;
        }
    }
}