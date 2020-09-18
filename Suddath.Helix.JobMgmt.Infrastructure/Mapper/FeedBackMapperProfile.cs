using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.RequestModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class FeedBackMapperProfile : Profile
    {
        public FeedBackMapperProfile()
        {
            CreateMap<Feedback, CreateFeedbackDto>().ReverseMap();

            CreateMap<Feedback, GetFeedbackDto>().ReverseMap();

            CreateMap<Configuration, FeedbackConfigDto>()
                .ForMember(d => d.NotificationTime, opt => opt.MapFrom(src => src.ToastNotificationTime));
        }
    }
}
