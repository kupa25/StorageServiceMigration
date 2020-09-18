using AutoMapper;
using Suddath.Helix.Common.Extensions;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.MileStone;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.SurveyAndAuthorization;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class EventMapperProfile : Profile
    {
        public EventMapperProfile()
        {
            CreateMap<Job, JobCreatedIntegrationEvent>()
                   .ForMember(d => d.Id, opt => opt.Ignore())
                   .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.AccountName, opt => opt.MapFrom(src => src.AccountEntity.Name))
                   .ForMember(d => d.TransfereeFirstName, opt => opt.MapFrom(src => src.Transferee.FirstName))
                   .ForMember(d => d.TransfereeLastName, opt => opt.MapFrom(src => src.Transferee.LastName))
                   .ForMember(d => d.Origin, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.OriginAddress)))
                   .ForMember(d => d.Destination, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.DestinationAddress)))
                   .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.OriginAddress.Country))
                   .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.DestinationAddress.Country))
                   .ForMember(d => d.IsVIP, opt => opt.MapFrom(src => src.Transferee.IsVip))
                   .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.Transferee, opt => opt.Ignore())
                   .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.BranchName))
                   ;

            CreateMap<Job2Dto, TransfereeCreatedIntegrationEvent>()
               .ForMember(d => d.Id, opt => opt.Ignore())
               .ForMember(d => d.TransfereeId, opt => opt.MapFrom(src => src.Transferee.Id))
               .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.Transferee.FirstName))
               .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.Transferee.LastName))
               .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Transferee.Emails.OrderBy(e => e.Id).FirstOrDefault().Value))
               .ForMember(d => d.Phone, opt => opt.MapFrom(src => src.Transferee.OriginPhones
                    .OrderByDescending(p => p.Primary)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new { Phone = string.Concat(p.DialCode, p.NationalNumber) })
                    .FirstOrDefault().Phone))
               .ForMember(d => d.Address1, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().Address1))
               .ForMember(d => d.Address2, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().Address2))
               .ForMember(d => d.Address3, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().Address3))
               .ForMember(d => d.City, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().City))
               .ForMember(d => d.State, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().State))
               .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().PostalCode))
               .ForMember(d => d.AdditionalAddressInfo, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().AdditionalAddressInfo))
               .ForMember(d => d.Country, opt => opt.MapFrom(src => src.JobInfo.Addresses.Where(a => a.Type == "Destination").FirstOrDefault().Country));

            CreateMap<JobSurveyInfo, JobSurveyInfoCreatedUpdatedIntegrationEvent>()
              .ForMember(d => d.Id, opt => opt.Ignore())
              .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.JobId))
              .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.Job.MoveType))
              .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.Job.OriginAddress.Country.IsUSA() ? "usa" : src.Job.OriginAddress.Country.ToLowerClean()))
              .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.Job.DestinationAddress.Country.ToLowerClean()))
              .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.JobId))
              .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.Job.BranchName))
              ;

            CreateMap<ServiceOrderMoveInfo, OriginAgentCreatedUpdatedIntegrationEvent>()
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                            src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                            src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.ActualPackDate, opt => opt.MapFrom(src => src.ActualPackStartDate))
                .ForMember(d => d.ActualPickupDate, opt => opt.MapFrom(src => src.ActualPickupStartDate))
                .ForMember(d => d.EstimatedPackDate, opt => opt.MapFrom(src => src.EstimatedPackStartDate))
                .ForMember(d => d.OASITDate, opt => opt.MapFrom(src => src.SITInDate))
                .ForMember(d => d.GreenLightDate, opt => opt.MapFrom(src => src.OAGreenLightDate))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
                .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
                .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.Id))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderDestinationAgent, DestinationAgentCreatedUpdatedIntegrationEvent>()
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                            src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                            src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.DASITDate, opt => opt.MapFrom(src => src.SITInDate))
                .ForMember(d => d.ScheduledDeliveryDate, opt => opt.MapFrom(src => src.ScheduledDeliveryStartDate))
                .ForMember(d => d.ActualDeliveryDate, opt => opt.MapFrom(src => src.ActualDeliveryStartDate))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
                .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
                .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.Id))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderAirFreightLeg, FreightLegServiceCreatedUpdatedIntegrationEvent>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                            src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                            src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.ActualDepartureDate, opt => opt.MapFrom(src => src.ActualDepartureDate))
                .ForMember(d => d.EstimatedDepartureDate, opt => opt.MapFrom(src => src.EstimatedDepartureDate))
                .ForMember(d => d.EstimatedArrivalDate, opt => opt.MapFrom(src => src.EstimatedArrivalDate))
                .ForMember(d => d.ActualArrivalDate, opt => opt.MapFrom(src => src.ActualArrivalDate))
                .ForMember(d => d.MasterAwbNumber, opt => opt.MapFrom(src => src.ServiceOrder.ServiceOrderAirFreight.MasterAwbNumber))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
                .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
                .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.Id))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreightLeg, FreightLegServiceCreatedUpdatedIntegrationEvent>()
               .ForMember(d => d.Id, opt => opt.Ignore())
               .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                            src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                            src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
               .ForMember(d => d.ActualDepartureDate, opt => opt.MapFrom(src => src.ActualDepartureDate))
               .ForMember(d => d.EstimatedDepartureDate, opt => opt.MapFrom(src => src.EstimatedDepartureDate))
               .ForMember(d => d.EstimatedArrivalDate, opt => opt.MapFrom(src => src.EstimatedArrivalDate))
               .ForMember(d => d.ActualArrivalDate, opt => opt.MapFrom(src => src.ActualArrivalDate))
               .ForMember(d => d.MasterBOLNumber, opt => opt.MapFrom(src => src.ServiceOrder.ServiceOrderOceanFreight.MasterBOLNumber))
               .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
               .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
               .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
               .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
               .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.Id))
               .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
               .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderRoadFreightLeg, FreightLegServiceCreatedUpdatedIntegrationEvent>()
              .ForMember(d => d.Id, opt => opt.Ignore())
              .ForMember(d => d.ServiceName,
                   opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                           src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                           src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
              .ForMember(d => d.ActualDepartureDate, opt => opt.MapFrom(src => src.ActualDepartureDate))
              .ForMember(d => d.EstimatedDepartureDate, opt => opt.MapFrom(src => src.EstimatedDepartureDate))
              .ForMember(d => d.EstimatedArrivalDate, opt => opt.MapFrom(src => src.EstimatedArrivalDate))
              .ForMember(d => d.ActualArrivalDate, opt => opt.MapFrom(src => src.ActualArrivalDate))
              .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
              .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
              .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
              .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
              .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
              .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreightContainer, FreighContainerCreatedUpdatedIntegrationEvent>()
              .ForMember(d => d.Id, opt => opt.Ignore())
              .ForMember(d => d.ContainerNumber, opt => opt.MapFrom(src => src.ContainerNumber))
              .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
              .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
              .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
              .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
              .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
              .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderAirFreight, FreightCreatedUpdatedIntegrationEvent>()
               .ForMember(d => d.Id, opt => opt.Ignore())
               .ForMember(d => d.ServiceName,
                   opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                           src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                           src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
               .ForMember(d => d.MasterAwbNumber, opt => opt.MapFrom(src => src.ServiceOrder.ServiceOrderAirFreight.MasterAwbNumber))
               .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
               .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
               .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
               .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
               .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.Id))
               .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
               .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreight, FreightCreatedUpdatedIntegrationEvent>()
               .ForMember(d => d.Id, opt => opt.Ignore())
               .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                            src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                            src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
               .ForMember(d => d.MasterBOLNumber, opt => opt.MapFrom(src => src.ServiceOrder.ServiceOrderOceanFreight.MasterBOLNumber))
               .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
               .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
               .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
               .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
               .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.Id))
               .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
               .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderRoadFreight, FreightCreatedUpdatedIntegrationEvent>()
              .ForMember(d => d.Id, opt => opt.Ignore())
              .ForMember(d => d.ServiceName,
                   opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                           src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                           src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
              .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
              .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.ServiceOrder.Job.MoveType.ToLowerClean()))
              .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.OriginAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.OriginAddress.Country.ToLowerClean()))
              .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.ServiceOrder.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.ServiceOrder.Job.DestinationAddress.Country.ToLowerClean()))
              .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
              .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<Invoice, InvoiceCreatedUpdatedIntegrationEvent>()
              .ForMember(d => d.Id, opt => opt.Ignore())
              .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.SuperServiceOrder.DisplayId))
              .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => DtoTranslations.ToCreateInvoiceResponse(src).SuperServiceName))
              .ForMember(d => d.ItemId, opt => opt.MapFrom(src => DtoTranslations.ToCreateInvoiceResponse(src).ServiceOrderId))
              .ForMember(d => d.VendorName, opt => opt.MapFrom(src => DtoTranslations.ToCreateInvoiceResponse(src).BillToName))
              .ForMember(d => d.InvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
              .ForMember(d => d.VendorAccountingId, opt => opt.MapFrom(src => DtoTranslations.ToCreateInvoiceResponse(src).VendorAccountingId))
              .ForMember(d => d.VendorShortAddress, opt => opt.MapFrom(src => DtoTranslations.ToCreateInvoiceResponse(src).VendorShortAddress));

            CreateMap<VendorInvoice, InvoiceCreatedUpdatedIntegrationEvent>()
             .ForMember(d => d.Id, opt => opt.Ignore())
             .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.SuperServiceOrder.DisplayId))
             .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => DtoTranslations.ToCreateVendorInvoiceResponse(src).SuperServiceName))
             .ForMember(d => d.ItemId, opt => opt.MapFrom(src => DtoTranslations.ToCreateVendorInvoiceResponse(src).ServiceOrderId))
             .ForMember(d => d.VendorName, opt => opt.MapFrom(src => DtoTranslations.ToCreateVendorInvoiceResponse(src).BillFromName))
             .ForMember(d => d.InvoiceNumber, opt => opt.MapFrom(src => src.VendorInvoiceNumber))
             .ForMember(d => d.VendorAccountingId, opt => opt.MapFrom(src => DtoTranslations.ToCreateVendorInvoiceResponse(src).VendorAccountingId))
             .ForMember(d => d.VendorShortAddress, opt => opt.MapFrom(src => DtoTranslations.ToCreateVendorInvoiceResponse(src).VendorShortAddress));

            CreateMap<VendorIntegrationEvent, VendorDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.VendorId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(d => d.Accounting_SI_Code, opt => opt.MapFrom(src => src.AccountingId));

            CreateMap<SuperServiceOrderSurveyResult, JobSurveyResultCreatedUpdatedIntegrationEvent>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.SuperServiceOrder.JobId))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.SuperServiceOrder.DisplayId))
                .ForMember(d => d.ServiceName,
                   opt => opt.MapFrom(src => src.SuperServiceOrder.SuperServiceModeId == null ?
                           src.SuperServiceOrder.SuperService.SuperServiceName :
                           src.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.SurveyedGrossWeightLb, opt => opt.MapFrom(src => src.GrossWeightLb))
                .ForMember(d => d.SurveyedNetWeightLb, opt => opt.MapFrom(src => src.NetWeightLb))
                .ForMember(d => d.AuthorizedGrossWeightLb,
                    opt => opt.MapFrom(src => DtoTranslations.FilterAuthorization(src.SuperServiceOrder.SuperServiceId,
                            MeasurementType.GROSS_LBS,
                            src.SuperServiceOrder.Job.JobSuperServiceAuthorization).Amount))
                .ForMember(d => d.AuthorizedNetWeightLb,
                    opt => opt.MapFrom(src => DtoTranslations.FilterAuthorization(src.SuperServiceOrder.SuperServiceId,
                            MeasurementType.NET_LBS,
                            src.SuperServiceOrder.Job.JobSuperServiceAuthorization).Amount))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.SuperServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderAirFreightItem, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrderId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrder.Service.ServiceName))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreightLiftVan, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrderId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.Service.ServiceName))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.JobId))
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                        src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                        src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreightLooseItem, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrderId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.Service.ServiceName))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.JobId))
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                        src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                        src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrderOceanFreightContainer.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreightLCL, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrderId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrder.Service.ServiceName))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                        src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                        src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderOceanFreightVehicle, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrderId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrder.Service.ServiceName))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.ServiceName,
                    opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperServiceModeId == null ?
                        src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName :
                        src.ServiceOrder.SuperServiceOrder.SuperServiceMode.ModeName))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<ServiceOrderRoadFreightLTL, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.SuperServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrderId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrder.Service.ServiceName))
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.ServiceOrder.JobId))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.SuperService.SuperServiceName))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.ServiceOrder.SuperServiceOrder.DisplayId))
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.ServiceOrder.Job.BranchName));

            CreateMap<GetJobCostMetricsResponse, ShipmentMetricsUpdatedIntegrationEvent>()
                .ForMember(d => d.ActualGrossWeightLb, opt => opt.MapFrom(src => src.GrossWeightLb))
                .ForMember(d => d.ActualNetWeightLb, opt => opt.MapFrom(src => src.NetWeightLb))
                .ForMember(d => d.SurveyedGrossWeightLb, opt => opt.MapFrom(src => src.SurveyGrossWeightLb))
                .ForMember(d => d.SurveyedNetWeightLb, opt => opt.MapFrom(src => src.SurveyNetWeightLb))
                .ForMember(d => d.OverweightPercentage, opt => opt.MapFrom(src => src.OverweightPercentage));

            CreateMap<JobContact, MoveContactUpdatedIntegrationEvent>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(d => d.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.Job.MoveType.ToLowerClean()))
                .ForMember(d => d.OriginCountry, opt => opt.MapFrom(src => src.Job.OriginAddress.Country.IsUSA() ? "usa" : src.Job.OriginAddress.Country.ToLowerClean()))
                .ForMember(d => d.DestinationCountry, opt => opt.MapFrom(src => src.Job.DestinationAddress.Country.IsUSA() ? "usa" : src.Job.DestinationAddress.Country.ToLowerClean()))
                .ForMember(d => d.DisplayId, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.Job.BranchName));
        }
    }
}