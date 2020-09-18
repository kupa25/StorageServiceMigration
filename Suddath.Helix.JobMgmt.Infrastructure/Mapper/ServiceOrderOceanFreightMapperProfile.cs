using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderOceanFreight;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderOceanFreightMapperProfile : Profile
    {
        public ServiceOrderOceanFreightMapperProfile()
        {
            CreateMap<ServiceOrderOceanFreight, ServiceOrderBaseResponse>()
               .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
               .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
               .ForPath(d => d.CarrierVendorName, opt => opt.MapFrom(src => src.CarrierVendor.Name))
               .ForPath(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
               .ForPath(d => d.SuperServiceName, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName))
               .ForPath(d => d.SuperServiceModeName, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
               .Include<ServiceOrderOceanFreight, GetServiceOrderOceanFreightResponse>();

            CreateMap<GetServiceOrderOceanFreightResponse, ServiceOrderOceanFreight>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.CarrierVendorId, opt => opt.MapFrom(src => src.CarrierVendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber))
               .ForMember(d => d.CarrierVendor, y => y.Ignore());

            CreateMap<ServiceOrderOceanFreight, GetServiceOrderOceanFreightResponse>()
               .ForPath(d => d.HouseNVOCCBOLNumber, opt => opt.MapFrom(src => src.HouseNOVCCBOLNumber));

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderOceanFreightResponse>().ReverseMap();

            CreateMap<ServiceOrderOceanFreightLeg, GetOceanFreightLegResponse>().ReverseMap();
            CreateMap<ServiceOrderOceanFreightLCL, GetOceanFreightLCLResponse>().ReverseMap();
            CreateMap<GetOceanFreightLegResponse, GetOceanFreightLegResponse>();
            CreateMap<GetOceanFreightLCLResponse, GetOceanFreightLCLResponse>();

            CreateMap<CreateOceanFreightLegRequest, ServiceOrderOceanFreightLeg>();

            CreateMap<ServiceOrderOceanFreightContainer, GetOceanFreightContainerResponse>()
                .ForPath(d => d.LiftVans, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightLiftVan))
                .ForPath(d => d.LooseItems, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightLooseItem))
                .ForPath(d => d.Vehicles, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightVehicle));

            CreateMap<GetOceanFreightContainerResponse, ServiceOrderOceanFreightContainer>();

            CreateMap<GetOceanFreightContainerResponse, GetOceanFreightContainerResponse>();

            CreateMap<ServiceOrderOceanFreightLooseItem, GetOceanFreightContainerLooseItemResponse>()
                .ForMember(d => d.OceanFreightContainerId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainerId))
                .ReverseMap();
            CreateMap<ServiceOrderOceanFreightLiftVan, GetOceanFreightContainerLiftVanResponse>()
                .ForMember(d => d.OceanFreightContainerId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainerId))
                .ReverseMap();
            CreateMap<ServiceOrderOceanFreightVehicle, GetOceanFreightContainerVehicleResponse>()
                .ForMember(d => d.OceanFreightContainerId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainerId))
                .ReverseMap();

            CreateMap<ServiceOrderOceanFreightVehicle, GetOceanFreightVehicleResponse>().ReverseMap();
            CreateMap<GetOceanFreightVehicleResponse, GetOceanFreightVehicleResponse>();
        }
    }
}