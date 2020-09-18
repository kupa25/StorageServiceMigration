using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public class ActivityMapperProfile : Profile
    {
        public ActivityMapperProfile()
        {
            CreateMap<Activity, GetTaskOrderActivityResponse>()
                .ForMember(d => d.ActivityTypeCode, opt => opt.MapFrom(src => src.ActivityTypeCode))
                .ForMember(d => d.ActivityId, opt => opt.MapFrom(src => src.Id))
                ;
        }
    }
}