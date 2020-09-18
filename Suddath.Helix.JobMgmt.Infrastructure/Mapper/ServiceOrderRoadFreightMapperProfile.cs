using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.RequestModels.ServiceOrderRoadFreight;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderRoadFreight;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderRoadFreightMapperProfile : Profile
    {
        public ServiceOrderRoadFreightMapperProfile()
        {
            CreateMap<ServiceOrderRoadFreight, ServiceOrderBaseResponse>()
               .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.CarrierVendor.Id))
               .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
               .ForPath(d => d.CarrierVendorName, opt => opt.MapFrom(src => src.CarrierVendor.Name))
               .ForPath(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
               .Include<ServiceOrderRoadFreight, GetServiceOrderRoadFreightResponse>();

            CreateMap<GetServiceOrderRoadFreightResponse, ServiceOrderRoadFreight>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.CarrierVendorId, opt => opt.MapFrom(src => src.CarrierVendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber))
               .ForMember(d => d.CarrierVendor, y => y.Ignore())
              ;

            CreateMap<ServiceOrderRoadFreight, GetServiceOrderRoadFreightResponse>()
               ;

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderRoadFreightResponse>().ReverseMap();

            CreateMap<ServiceOrderRoadFreightLeg, GetRoadFreightLegResponse>().ReverseMap();
            CreateMap<GetRoadFreightLegResponse, GetRoadFreightLegResponse>();

            CreateMap<CreateRoadFreightLegRequest, ServiceOrderRoadFreightLeg>();

            CreateMap<ServiceOrderRoadFreightLTL, GetRoadFreightLTLResponse>().ReverseMap();
           
        }
    }
}