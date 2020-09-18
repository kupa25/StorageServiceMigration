using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;

namespace Suddath.Helix.JobMgmt.Services.Water.Mapper
{
    public class MoveMapperProfile : AutoMapper.Profile
    {
        public MoveMapperProfile()
        {
            CreateMap<Move, Job2Dto>()
                .ForMember(dest => dest.Job, opt => opt.MapFrom((src, d) => MoveMappers.ToJob(src)))
                .ForMember(dest => dest.JobInfo, opt => opt.MapFrom((src, d) => MoveMappers.ToJobInfo(src)))
                .ForMember(dest => dest.Transferee, opt => opt.MapFrom(src => MoveMappers.ToTransferee(src)));
            ;

            CreateMap<Move, JobDto>()
                .ForMember(d => d.AccountLabel, opt => opt.MapFrom(src => "FAKE"))
                .ForMember(d => d.BillToLabel, opt => opt.MapFrom(src => "FAKE"))
                .ForMember(d => d.Status, opt => opt.MapFrom(src => "active-(booked)"))

                .ForPath(d => d.BillTo.Id, opt => opt.MapFrom(src => 1))
                .ForPath(d => d.Account.Id, opt => opt.MapFrom(src => 1))
                .ForPath(d => d.Booker.Id, opt => opt.MapFrom(src => 1))

                .ForMember(d => d.RevenueType, opt => opt.MapFrom(src => "House Account"))
                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => "Door-to-Door"))
                .ForMember(d => d.AuthPoNum, opt => opt.MapFrom(src => src.PONumber))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => MoveMappers.ToBranchName(src.BRANCH_CODE)))
                ;
        }
    }
}