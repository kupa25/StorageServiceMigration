using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using System;
using System.Reflection;

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

                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => "Door-to-Door"))
                .ForMember(d => d.AuthPoNum, opt => opt.MapFrom(src => src.PONumber))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => MoveMappers.ToBranchName(src.RegNumber, src.BRANCH_CODE)))
                ;

            CreateMap<Notes, CreateJobNoteRequest>()
                .ForMember(d => d.Module, opt => opt.MapFrom(src => "JOB"))
                .ForMember(d => d.Message, opt => opt.MapFrom(src => src.NOTE))
                .ForMember(d => d.CreatedBy, opt => opt.MapFrom(src => src.ENTERED_BY))
                .ForMember(d => d.Category, opt => opt.MapFrom(src => MoveMappers.ToNotesCategory(src)))
                .ForMember(d => d.IsCritical, opt => opt.MapFrom(src => src.CATEGORY.Equals("CRITICAL")))
                .ForMember(d => d.DateCreated, opt => opt.MapFrom(src => src.DATE_ENTERED))
                ;

            CreateMap<Notes, CreateJobTaskRequest>()
                .ForMember(d => d.Subject, opt => opt.MapFrom(src => src.NOTE))
                .ForMember(d => d.CreatedBy, opt => opt.MapFrom(src => src.ENTERED_BY))

                .ForMember(d => d.IsCritical, opt => opt.MapFrom(src => src.CATEGORY.Equals("CRITICAL")))

                ;

            CreateMap<Notes, Models.WorkflowTask>()
                .ForMember(d => d.ReferenceId, opt => opt.MapFrom(src => src.JobId))
                .ForMember(d => d.Module, opt => opt.MapFrom(src => "JOB"))
                .ForMember(d => d.DisplayId, opt => opt.MapFrom(src => src.JobId.ToString()))
                .ForMember(d => d.Subject, opt => opt.MapFrom(src => src.NOTE))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(src => src.CALLBACK_DATE))
                .ForMember(d => d.IsCritical, opt => opt.MapFrom(src => src.CATEGORY.Equals("CRITICAL")))
                .ForMember(d => d.AssignedTo, opt => opt.MapFrom(src => src.ENTERED_BY))
                .ForMember(d => d.CreatedBy, opt => opt.MapFrom(src => src.ENTERED_BY))
                .ForMember(d => d.ModifiedBy, opt => opt.MapFrom(src => src.ENTERED_BY))
                ;

            CreateMap<CreateJobNoteRequest, Infrastructure.Domain.Note>();
        }
    }
}