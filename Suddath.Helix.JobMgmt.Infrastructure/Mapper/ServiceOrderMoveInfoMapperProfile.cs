using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderMoveInfoMapperProfile : Profile
    {
        public ServiceOrderMoveInfoMapperProfile()
        {
            CreateMap<ServiceOrderMoveInfo, ServiceOrderBaseResponse>()
              .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
              .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
              .ForMember(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
              .Include<ServiceOrderMoveInfo, GetServiceOrderOriginAgentResponse>();

            CreateMap<GetServiceOrderOriginAgentResponse, ServiceOrderMoveInfo>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber))
               .ForPath(d => d.EstimatedPackStartTime, opt => opt.MapFrom(src => DtoTranslations.ToTimeSpan(src.EstimatedPackStartTime)))
               .ForPath(d => d.EstimatedPackEndTime, opt => opt.MapFrom(src => DtoTranslations.ToTimeSpan(src.EstimatedPackEndTime)))
               .ForPath(d => d.ServiceOrder.ISAllDocumentsReceived, opt => opt.MapFrom(src => src.IsAllDocumentsReceived))
               ;

            CreateMap<ServiceOrderMoveInfo, GetServiceOrderOriginAgentResponse>()
               .ForPath(d => d.EstimatedPackStartTime, opt => opt.MapFrom(src => DtoTranslations.ToDateTime(src.EstimatedPackStartDate, src.EstimatedPackStartTime)))
               .ForPath(d => d.EstimatedPackEndTime, opt => opt.MapFrom(src => DtoTranslations.ToDateTime(src.EstimatedPackEndDate, src.EstimatedPackEndTime)))
               .ForPath(d => d.IsAllDocumentsReceived, opt => opt.MapFrom(src => src.ServiceOrder.ISAllDocumentsReceived))
               ;

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderOriginAgentResponse>().ReverseMap();
        }
    }
}