using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class BillableItemMapperProfile : Profile
    {
        public BillableItemMapperProfile()
        {
            CreateMap<BillableItem, GetBillableItemResponse>()
                .ForMember(d => d.Status, opt =>
                        opt.MapFrom(src => src.BillableItemStatusIdentifier))
                .ForMember(d => d.BillToName, opt =>
                        opt.MapFrom(src => DtoTranslations.ToGetBillableItemResponse(src).BillToName))
                .ForMember(d => d.BillToType, opt =>
                        opt.MapFrom(src => DtoTranslations.ToGetBillableItemResponse(src).BillToType))
                 .ForMember(d => d.BillToId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToGetBillableItemResponse(src).BillToId))
                 .ForMember(d => d.BillToLabel, opt =>
                        opt.MapFrom(src => DtoTranslations.ToGetBillableItemResponse(src).BillToLabel))
                 .ForMember(d => d.BillableItemTypeName, opt => opt.MapFrom(src => src.BillableItemType.BillableItemTypeName))
                 .ForMember(d => d.WriteOffModifiedDateTime, opt => opt.MapFrom(src => src.WriteOffModifiedDateTime))
                 .ForMember(d => d.InvoiceNumber, opt => opt.MapFrom(src => src.Invoice.InvoiceNumber))
                 .ForMember(d => d.InvoiceDate, opt => opt.MapFrom(src => src.Invoice.InvoiceDate))
                 .ForMember(d => d.InvoiceType, opt => opt.MapFrom(src => src.Invoice.InvoiceType))
                 .ForMember(d => d.DepositDate, opt => opt.MapFrom(src => src.Invoice.LastPaidDate))
                ;

            CreateMap<GetBillableItemResponse, BillableItem>()
                .ForMember(d => d.BillToAccountEntityId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToBillableItem(src).BillToAccountEntityId))
                .ForMember(d => d.BillToVendorId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToBillableItem(src).BillToVendorId))
                .ForMember(d => d.BillToTransfereeId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToBillableItem(src).BillToTransfereeId))
                .ForMember(d => d.BillToType, opt =>
                        opt.MapFrom(src => DtoTranslations.ToBillableItem(src).BillToType))

                .ForMember(d => d.BillableItemStatusIdentifier, opt => opt.Ignore())
                .ForMember(d => d.SuperServiceOrderId, opt => opt.Ignore())
                ;

            CreateMap<BillableItem, CreateBillableItemResponse>()
                .ForMember(d => d.Status, opt =>
                        opt.MapFrom(src => src.BillableItemStatusIdentifier))
                .ForMember(d => d.BillToName, opt =>
                        opt.MapFrom(src => DtoTranslations.ToCreateBillableItemResponse(src).BillToName))
                 .ForMember(d => d.BillToId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToCreateBillableItemResponse(src).BillToId))
                 .ForMember(d => d.BillToType, opt =>
                        opt.MapFrom(src => DtoTranslations.ToCreateBillableItemResponse(src).BillToType))
                ;

            CreateMap<BillableItemType, GetBillableItemTypeResponse>()
                .ForMember(d => d.Name, opt =>
                        opt.MapFrom(src => string.Concat(src.AccountCode, "-", src.BillableItemTypeName)))
                .ForMember(d => d.Value, opt =>
                        opt.MapFrom(src => src.Id.ToString()))
               ;
        }
    }
}