using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderDestinationAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class ServiceOrderDestinationAgentMapperProfile : Profile
    {
        public ServiceOrderDestinationAgentMapperProfile()
        {
            CreateMap<GetServiceOrderDestinationAgentResponse, GetServiceOrderDestinationAgentResponse>();

            CreateMap<ServiceOrderDestinationAgent, ServiceOrderBaseResponse>()
               .ForPath(d => d.VendorId, opt => opt.MapFrom(src => src.ServiceOrder.VendorId))
               .ForPath(d => d.VendorName, opt => opt.MapFrom(src => src.ServiceOrder.Vendor.Name))
               .ForMember(d => d.QuoteReferenceNumber, opt => opt.MapFrom(src => src.ServiceOrder.QuoteReferenceNumber))
               .Include<ServiceOrderDestinationAgent, GetServiceOrderDestinationAgentResponse>();

            CreateMap<GetServiceOrderDestinationAgentResponse, ServiceOrderDestinationAgent>()
               .ForPath(d => d.ServiceOrder.VendorId, opt => opt.MapFrom(src => src.VendorId))
               .ForPath(d => d.ServiceOrder.QuoteReferenceNumber, opt => opt.MapFrom(src => src.QuoteReferenceNumber))
               .ForPath(d => d.ScheduledDeliveryStartTime, opt => opt.MapFrom(src => DtoTranslations.ToTimeSpan(src.ScheduledDeliveryStartTime)))
               .ForPath(d => d.ScheduledDeliveryEndTime, opt => opt.MapFrom(src => DtoTranslations.ToTimeSpan(src.ScheduledDeliveryEndTime)));

            CreateMap<ServiceOrderDestinationAgent, GetServiceOrderDestinationAgentResponse>()
            .ForPath(d => d.ScheduledDeliveryStartTime, opt => opt.MapFrom(src => DtoTranslations.ToDateTime(src.ScheduledDeliveryStartDate, src.ScheduledDeliveryStartTime)))
            .ForPath(d => d.ScheduledDeliveryEndTime, opt => opt.MapFrom(src => DtoTranslations.ToDateTime(src.ScheduledDeliveryEndDate, src.ScheduledDeliveryEndTime)))
            .ForPath(d => d.IsPartialDeliveryExists, opt => opt.MapFrom(src => CheckIfPartialDeliveryExists(src.ServiceOrder)));

            CreateMap<ServiceOrderDestinationAgentPartialDelivery, GetDestinationAgentPartialDeliveryResponse>().ReverseMap();

            CreateMap<GetDestinationAgentPartialDeliveryResponse, GetDestinationAgentPartialDeliveryResponse>();
        }

        private object CheckIfPartialDeliveryExists(ServiceOrder serviceOrder)
        {
            var result = false;

            if (serviceOrder != null &&
                        serviceOrder.ServiceOrderDestinationAgentPartialDelivery != null &&
                        serviceOrder.ServiceOrderDestinationAgentPartialDelivery.Count > 0)
            {
                result = true;

                //TODO: cordinate with UI Team to fix this.  Currently they are posting a blank record
                var firstPartialDelivery = serviceOrder.ServiceOrderDestinationAgentPartialDelivery.First();

                if (serviceOrder.ServiceOrderDestinationAgentPartialDelivery.Count == 1)
                {
                    if (
                        !firstPartialDelivery.PartialDeliveryDate.HasValue &&
                        !firstPartialDelivery.WeightDeliveredLb.HasValue)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
    }
}