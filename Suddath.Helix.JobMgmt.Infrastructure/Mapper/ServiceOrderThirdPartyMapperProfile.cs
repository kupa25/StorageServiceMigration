using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderThirdParty;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderThirdPartyMapperProfile : Profile
    {
        public ServiceOrderThirdPartyMapperProfile()
        {
            CreateMap<ServiceOrderThirdParty, ServiceOrderBaseResponse>()
                .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
                .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
                .ForMember(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
                .Include<ServiceOrderThirdParty, GetServiceOrderThirdPartyResponse>();

            CreateMap<GetServiceOrderThirdPartyResponse, ServiceOrderThirdParty>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber));

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderThirdPartyResponse>().ReverseMap();

            CreateMap<ServiceOrderThirdParty, GetServiceOrderThirdPartyResponse>();

            CreateMap<GetServiceOrderThirdPartyResponse, GetServiceOrderThirdPartyResponse>();

            CreateMap<GetThirdPartyCrateResponse, ServiceOrderThirdPartyCrate>().ReverseMap();
            CreateMap<GetThirdPartyServiceResponse, ServiceOrderThirdPartyService>().ReverseMap();

            CreateMap<GetThirdPartyServiceResponse, GetThirdPartyServiceResponse>();
            CreateMap<GetThirdPartyCrateResponse, GetThirdPartyCrateResponse>();
        }
    }
}