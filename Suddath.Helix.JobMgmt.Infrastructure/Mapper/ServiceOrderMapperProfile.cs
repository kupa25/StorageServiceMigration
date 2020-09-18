using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderDestinationAgent;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderOceanFreight;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderMapperProfile : Profile
    {
        public ServiceOrderMapperProfile()
        {
            CreateMap<ServiceOrderContact, GetServiceOrderContactResponse>().ReverseMap();

            CreateMap<GetServiceOrderContactResponse, GetServiceOrderContactResponse>();

            CreateMap<CreateServiceOrderContactRequest, ServiceOrderContact>();

            CreateMap<ServiceOrderContact, CreateServiceOrderContactResponse>();

            CreateMap<SuperServiceOrderSurveyResult, GetSurveyResultResponse>()
                .ForPath(d => d.Accessorials, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperServiceOrderAccessorial.Where(x => x.AccessorialName != null)))
                .ForPath(d => d.ThirdParyServices, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperServiceOrderAccessorial.Where(x => x.ThirdPartyServiceName != null)))
                .ForPath(d => d.Crates, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperServiceOrderCrate))
                .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperService.SuperServiceName))
                .ForMember(d => d.SuperServiceId, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperServiceId))
                .ForMember(d => d.SuperServiceIconName, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperService.SuperServiceIconName))
                .ForMember(d => d.SuperServiceModeName, opt => opt.MapFrom(src => src.SuperServiceOrder.SuperServiceMode.ModeName));

            CreateMap<GetSurveyResultResponse, SuperServiceOrderSurveyResult>()
                .ForMember(d => d.SuperServiceOrder, opt => opt.Ignore());

            CreateMap<CreateSurveyResultAccessorialRequest, SuperServiceOrderAccessorial>();
            CreateMap<CreateSurveyResultThirdPartyServiceRequest, SuperServiceOrderAccessorial>();
            CreateMap<CreateSurveyResultCrateRequest, SuperServiceOrderCrate>();

            CreateMap<SuperServiceOrderAccessorial, GetSurveyResultAccessorialResponse>().ReverseMap();
            CreateMap<SuperServiceOrderAccessorial, GetSurveyResultThirdPartyServiceResponse>().ReverseMap();
            CreateMap<SuperServiceOrderCrate, GetSurveyResultCrateResponse>().ReverseMap();

            CreateMap<Vendor, GetSuperServiceOrderAvailableBillTosResponse>()
                .ForMember(d => d.BillToType, opt => opt.MapFrom(src => EntityType.VENDOR))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.AccountingId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => EntityType.VENDOR))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => $"{EntityType.VENDOR}-{src.Id}"))
               ;

            CreateMap<AccountEntity, GetSuperServiceOrderAvailableBillTosResponse>()
                .ForMember(d => d.BillToType, opt => opt.MapFrom(src => EntityType.ACCOUNT_ENTITY))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.AccountingId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => EntityType.ACCOUNT_ENTITY))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => $"{EntityType.ACCOUNT_ENTITY}-{src.Id}"))
               ;

            CreateMap<Transferee, GetSuperServiceOrderAvailableBillTosResponse>()
                .ForMember(d => d.BillToType, opt => opt.MapFrom(src => EntityType.TRANSFEREE))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.AccountingId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => string.Concat(src.FirstName, " ", src.LastName)))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => EntityType.TRANSFEREE))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => $"{EntityType.TRANSFEREE}-{src.Id}"))
               ;
        }
    }
}