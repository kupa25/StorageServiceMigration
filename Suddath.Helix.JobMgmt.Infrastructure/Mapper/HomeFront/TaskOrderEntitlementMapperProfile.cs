using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public class TaskOrderEntitlementMapperProfile : Profile
    {
        public TaskOrderEntitlementMapperProfile()
        {
            // Based on discussion regarding the future data modelling of these,
            // these will likely expand to pull from multiple tables to
            // put into and pull out of each so I put them in their own profile.
            CreateMap<Entitlement, GetTaskOrderEntitlementResponse>();
            CreateMap<GetTaskOrderEntitlementResponse, Entitlement>();
        }
    }
}