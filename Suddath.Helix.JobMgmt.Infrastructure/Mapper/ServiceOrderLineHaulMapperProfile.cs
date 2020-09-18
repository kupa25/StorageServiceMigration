using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderLineHaulMapperProfile : Profile
    {
        public ServiceOrderLineHaulMapperProfile()
        {
            CreateMap<ServiceOrderLineHaul, ServiceOrderBaseResponse>()
             .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
             .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
             .ForMember(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
             .Include<ServiceOrderLineHaul, GetServiceOrderLineHaulResponse>();

            CreateMap<GetServiceOrderLineHaulResponse, ServiceOrderLineHaul>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber));

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderLineHaulResponse>().ReverseMap();

            CreateMap<ServiceOrderLineHaul, GetServiceOrderLineHaulResponse>();
            CreateMap<GetServiceOrderLineHaulResponse, GetServiceOrderLineHaulResponse>();
        }
    }
}