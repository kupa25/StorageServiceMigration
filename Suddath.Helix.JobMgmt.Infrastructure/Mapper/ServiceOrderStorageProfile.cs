using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderStorageProfile : Profile
    {
        public ServiceOrderStorageProfile()
        {
            CreateMap<ServiceOrderStorage, GetServiceOrderStorageResponse>()
                .ForMember(d => d.Currency, opt => opt.MapFrom(src => src.StorageCurrency))
                .ForMember(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
                .ForMember(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
                ;

            CreateMap<GetServiceOrderStorageResponse, ServiceOrderStorage>()
                .ForMember(d => d.StorageCurrency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(d => d.InsuranceCurrency, opt => opt.MapFrom(src => src.Currency))
                .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
                ;

            CreateMap<ServiceOrderStoragePartialDelivery, GetStoragePartialDeliveryResponse>()
                .ForMember(d => d.User, opt => opt.MapFrom(src => src.UserName))
                .ForMember(d => d.IsEditable, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ReferenceType)))
                .ForMember(d => d.Module, opt => opt.MapFrom(src => src.ReferenceType))
                ;

            CreateMap<ServiceOrderStorageRevenue, GetStorageRevenueResponse>()
                .ForMember(d => d.BillToId, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageResponse(src).BillToId))
                .ForMember(d => d.BillToType, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageResponse(src).BillToType))
                .ForMember(d => d.BillToName, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(DtoTranslations.ToBillToStorageResponse(src).BillToName) ?
                        "Bill To" : DtoTranslations.ToBillToStorageResponse(src).BillToName))
                .ForMember(d => d.BillToLabel, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageResponse(src).BillToLabel))
                ;

            CreateMap<GetStorageRevenueResponse, ServiceOrderStorageRevenue>()
                .ForMember(d => d.BillToAccountEntityId, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageRevenue(src).BillToAccountEntityId))
                .ForMember(d => d.BillToVendorId, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageRevenue(src).BillToVendorId))
                .ForMember(d => d.BillToTransfereeId, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageRevenue(src).BillToTransfereeId))
                .ForMember(d => d.BillToType, opt => opt.MapFrom(src => DtoTranslations.ToBillToStorageRevenue(src).BillToType))
                ;
        }
    }
}