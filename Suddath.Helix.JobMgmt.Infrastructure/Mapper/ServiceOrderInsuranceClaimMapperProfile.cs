using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.InsuranceClaim;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderInsuranceClaimMapperProfile : Profile
    {
        public ServiceOrderInsuranceClaimMapperProfile()
        {
            CreateMap<ServiceOrderInsuranceClaim, ServiceOrderBaseResponse>()
               .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
               .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
               .Include<ServiceOrderInsuranceClaim, GetServiceOrderInsuranceClaimResponse>();

            CreateMap<ServiceOrderInsuranceClaim, GetServiceOrderInsuranceClaimResponse>();

            CreateMap<GetServiceOrderInsuranceClaimResponse, ServiceOrderInsuranceClaim>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForMember(d => d.ClaimStatus, opt => opt.Ignore());
            ;

            CreateMap<ServiceOrderClaimRepair, GetClaimRepairResponse>()
                .ReverseMap();
            CreateMap<ServiceOrderClaimDamage, GetClaimDamageResponse>()
                .ReverseMap();
            CreateMap<ServiceOrderClaimInspection, GetClaimInspectionResponse>()
                .ReverseMap();
            CreateMap<ServiceOrderClaimSettlement, GetClaimSettlementResponse>()
                .ReverseMap();
        }
    }
}