using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderAirFreight;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderAirFreightMapperProfile : Profile
    {
        public ServiceOrderAirFreightMapperProfile()
        {
            CreateMap<ServiceOrderAirFreight, ServiceOrderBaseResponse>()
               .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Id))
               .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
               .ForPath(d => d.CarrierVendorName, opt => opt.MapFrom(src => src.CarrierVendor.Name))
               .ForPath(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
               .Include<ServiceOrderAirFreight, GetServiceOrderAirFreightResponse>();

            CreateMap<GetServiceOrderAirFreightResponse, ServiceOrderAirFreight>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.CarrierVendorId, opt => opt.MapFrom(src => src.CarrierVendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber))
               .ForMember(d => d.CarrierVendor, y => y.Ignore());

            CreateMap<ServiceOrderAirFreight, GetServiceOrderAirFreightResponse>()
                .ForMember(d => d.HouseAirwayBillNumber, opt => opt.MapFrom(src => src.HouseAirwayBillNumber))
                .ForMember(d => d.MasterAwbNumber, opt => opt.MapFrom(src => src.MasterAwbNumber));

            CreateMap<ServiceOrderBaseResponse, GetServiceOrderAirFreightResponse>().ReverseMap();

            CreateMap<ServiceOrderAirFreightLeg, GetAirFreightLegResponse>().ReverseMap();
            CreateMap<ServiceOrderAirFreightItem, GetAirFreightItemResponse>().ReverseMap();

            CreateMap<GetAirFreightLegResponse, GetAirFreightLegResponse>();
            CreateMap<GetAirFreightItemResponse, GetAirFreightItemResponse>();
        }
    }

    /// <summary>
    /// NEEDS TO BE USED IN THE VERY NEAR FUTURE
    /// </summary>
    public static class MapperExtensions
    {
        public static IMappingExpression<ServiceOrderAirFreight, TDestination> MapBase<TDestination>(
        this IMappingExpression<ServiceOrderAirFreight, TDestination> mapping)
        where TDestination : ServiceOrderBaseResponse
        {
            // all base class mappings goes here
            return mapping.ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.CarrierVendor.Id))
                          .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
                          .ForPath(d => d.CarrierVendorName, opt => opt.MapFrom(src => src.CarrierVendor.Name))
                          .ForPath(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber));
        }
    }
}