using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class AccountingMapperProfile : Profile
    {
        public AccountingMapperProfile()
        {
            CreateMap<GLCode, GetGLCodeResponse>()
                .ForMember(d => d.GLCode, opt =>
                     opt.MapFrom(src => src.GLCode1))
                ;
        }
    }
}