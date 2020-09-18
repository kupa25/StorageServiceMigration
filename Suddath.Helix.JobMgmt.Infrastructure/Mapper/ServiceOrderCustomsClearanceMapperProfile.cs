using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderCustomsClearanceMapperProfile : Profile
    {
        public ServiceOrderCustomsClearanceMapperProfile()
        {
            CreateMap<ServiceOrderCustomClearance, ServiceOrderBaseResponse>()
                .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
                .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
                .ForMember(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
                .Include<ServiceOrderCustomClearance, GetServiceOrderCustomsClearanceResponse>();

            CreateMap<GetServiceOrderCustomsClearanceResponse, ServiceOrderCustomClearance>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber));

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderCustomsClearanceResponse>().ReverseMap();

            CreateMap<ServiceOrderCustomClearance, GetServiceOrderCustomsClearanceResponse>();
            CreateMap<GetServiceOrderCustomsClearanceResponse, GetServiceOrderCustomsClearanceResponse>();
        }
    }
}