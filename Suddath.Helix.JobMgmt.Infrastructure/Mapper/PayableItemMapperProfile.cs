using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class PayableItemMapperProfile : Profile
    {
        public PayableItemMapperProfile()
        {
            CreateMap<PayableItem, GetPayableItemResponse>()
                .ForMember(d => d.Status, opt =>
                        opt.MapFrom(src => src.PayableItemStatusIdentifier))
                 .ForMember(d => d.PayableItemTypeName, opt =>
                        opt.MapFrom(src => src.BillableItemType.BillableItemTypeName))
                 .ForMember(d => d.PayableItemTypeId, opt =>
                        opt.MapFrom(src => src.BillableItemTypeId))
                .ForMember(d => d.VendorInvoiceType, opt => opt.MapFrom(src => src.VendorInvoice.VendorInvoiceType))
                .ForMember(d => d.VendorInvoiceDate, opt => opt.MapFrom(src => src.VendorInvoiceDate ?? src.VendorInvoice.VendorInvoiceDate))
                .ForMember(d => d.VendorInvoiceNumber, opt => opt.MapFrom(src => src.VendorInvoiceNumber ?? src.VendorInvoice.VendorInvoiceNumber))
                .ForMember(d => d.PaidDate, opt => opt.MapFrom(src => src.VendorInvoice.LastPaidDate))
                .ForMember(d => d.CheckWireNumber, opt => opt.MapFrom(src => src.VendorInvoice.LastPaidCheckNumber))
                .ForMember(d => d.BillFromName, opt => opt.MapFrom(src => DtoTranslations.ToGetPayableItemResponse(src).BillFromName))
                .ForMember(d => d.BillFromType, opt => opt.MapFrom(src => DtoTranslations.ToGetPayableItemResponse(src).BillFromType))
                .ForMember(d => d.BillFromLabel, opt => opt.MapFrom(src => DtoTranslations.ToGetPayableItemResponse(src).BillFromLabel))
                .ForMember(d => d.BillFromId, opt => opt.MapFrom(src => DtoTranslations.ToGetPayableItemResponse(src).BillFromId));

            CreateMap<GetPayableItemResponse, PayableItem>()
                .ForMember(d => d.BillableItemTypeId, opt => opt.MapFrom(src => src.PayableItemTypeId))
                .ForMember(d => d.PayableItemStatusIdentifier, opt => opt.Ignore())
                .ForMember(d => d.SuperServiceOrderId, opt => opt.Ignore())
                .ForMember(d => d.VendorInvoiceId, opt => opt.Ignore())
                .ForPath(d => d.VendorInvoice.LastPaidCheckNumber, opt => opt.Ignore())
                .ForPath(d => d.VendorInvoice.LastPaidDate, opt => opt.Ignore())
                .ForMember(d => d.BillFromAccountEntityId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToPayableItem(src).BillFromAccountEntityId))
                .ForMember(d => d.BillFromVendorId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToPayableItem(src).BillFromVendorId))
                .ForMember(d => d.BillFromTransfereeId, opt =>
                        opt.MapFrom(src => DtoTranslations.ToPayableItem(src).BillFromTransfereeId))
                .ForMember(d => d.BillFromType, opt =>
                        opt.MapFrom(src => DtoTranslations.ToPayableItem(src).BillFromType));

            CreateMap<PayableItem, CreatePayableItemResponse>()
                .ForMember(d => d.Status, opt =>
                        opt.MapFrom(src => src.PayableItemStatusIdentifier))
                .ReverseMap()
                ;

            CreateMap<BillableItemType, GetBillableItemTypeResponse>()
                .ForMember(d => d.Name, opt =>
                        opt.MapFrom(src => string.Concat(src.AccountCode, "-", src.BillableItemTypeName)))
                .ForMember(d => d.Value, opt =>
                        opt.MapFrom(src => src.Id.ToString()))
               ;

            CreateMap<Vendor, GetSuperServiceOrderAvailableBillFromResponse>()
                .ForMember(d => d.BillFromType, opt => opt.MapFrom(src => EntityType.VENDOR))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Accounting_SI_Code))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => EntityType.VENDOR))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => string.Concat(EntityType.VENDOR, "-", src.Id)))
   ;

            CreateMap<AccountEntity, GetSuperServiceOrderAvailableBillFromResponse>()
                .ForMember(d => d.BillFromType, opt => opt.MapFrom(src => EntityType.ACCOUNT_ENTITY))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.AccountingId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => EntityType.ACCOUNT_ENTITY))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => string.Concat(EntityType.ACCOUNT_ENTITY, "-", src.Id)))
               ;

            CreateMap<Transferee, GetSuperServiceOrderAvailableBillFromResponse>()
                .ForMember(d => d.BillFromType, opt => opt.MapFrom(src => EntityType.TRANSFEREE))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.AccountingId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => string.Concat(src.FirstName, " ", src.LastName)))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => EntityType.TRANSFEREE))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => string.Concat(EntityType.TRANSFEREE, "-", src.Id)))
                ;
        }
    }
}